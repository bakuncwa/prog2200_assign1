using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace prog2200_assign1_library
{
    /// <summary>
    /// Starts the chat server (NetCat / ncat) in its own command prompt window, so the server's
    /// connections and messages can be watched - and replied to - while the game runs.
    /// </summary>
    public sealed class ChatServerLauncher
    {
        /// <summary>How long to wait for a newly started server to begin listening.</summary>
        private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(5);

        private const string NcatFileName = "ncat.exe";

        // Guards against opening two server windows when Connect is clicked twice quickly.
        private readonly object _sync = new object();
        private Task<bool> _startTask;

        /// <summary>Raised when the server could not be started.</summary>
        public event ChatErrorEventHandler ErrorOccurred;

        /// <summary>
        /// Makes sure a server is listening on <paramref name="port"/>. If nothing is, a new command
        /// prompt window running "ncat -l -p [port] -t -v -k" is opened and this waits until it is ready.
        /// </summary>
        /// <returns>True once a server is listening; false if one could not be started.</returns>
        public Task<bool> EnsureServerRunningAsync(int port)
        {
            lock (_sync)
            {
                if (_startTask == null || _startTask.IsCompleted)
                {
                    _startTask = StartServerAsync(port);
                }

                return _startTask;
            }
        }

        /// <summary>True if any program on this computer is listening for TCP connections on the port.</summary>
        public static bool IsServerListening(int port)
        {
            return IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpListeners()
                .Any(endPoint => endPoint.Port == port);
        }

        private async Task<bool> StartServerAsync(int port)
        {
            // A server that is already running (started earlier, or by hand) is simply reused.
            if (IsServerListening(port))
            {
                return true;
            }

            string ncatPath = FindNcat();

            if (ncatPath == null)
            {
                OnErrorOccurred("The server could not be started - ncat was not found. Install Nmap "
                    + "(https://nmap.org/download), which includes ncat, then try again.", null);
                return false;
            }

            string serverCommand = "ncat -l -p " + port + " -t -v -k";

            // cmd /k keeps the window open after ncat stops, so its history stays visible.
            // -l listen, -p port, -t answer telnet negotiation, -v show connections,
            // -k keep listening after a client disconnects, so the game can reconnect.
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/k \"title Chat Server - " + serverCommand
                    + " & echo " + serverCommand
                    + " & \"" + ncatPath + "\" -l -p " + port + " -t -v -k\"",
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(ncatPath)
            };

            try
            {
                using (Process.Start(startInfo))
                {
                    // The window runs on its own; nothing more is needed from the process.
                }
            }
            catch (Win32Exception ex)
            {
                OnErrorOccurred("The server window could not be opened - " + ex.Message, ex);
                return false;
            }

            DateTime deadline = DateTime.Now + StartTimeout;

            while (DateTime.Now < deadline)
            {
                await Task.Delay(100).ConfigureAwait(false);

                if (IsServerListening(port))
                {
                    return true;
                }
            }

            OnErrorOccurred("The server window opened, but the server did not start listening on port "
                + port + ". Check the server window for details.", null);
            return false;
        }

        /// <summary>
        /// Looks for ncat.exe on the PATH, then in the folders the Nmap installer uses.
        /// </summary>
        private static string FindNcat()
        {
            string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

            var folders = path.Split(Path.PathSeparator)
                .Concat(new[]
                {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Nmap"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Nmap")
                });

            foreach (string folder in folders)
            {
                if (string.IsNullOrWhiteSpace(folder))
                {
                    continue;
                }

                try
                {
                    string candidate = Path.Combine(folder.Trim().Trim('"'), NcatFileName);

                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
                catch (ArgumentException)
                {
                    // A PATH entry with invalid characters - skip it.
                }
            }

            return null;
        }

        private void OnErrorOccurred(string message, Exception exception)
        {
            ErrorOccurred?.Invoke(this, new ChatErrorEventArgs(message, exception));
        }
    }
}
