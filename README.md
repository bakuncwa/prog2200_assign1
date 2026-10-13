# PROG2200 Assignment 1 – Asynchronous Multi-Threaded Chat Client

A Windows Forms "Network Game" whose chat panel talks to a TCP server asynchronously.
All networking lives in a separate class library; ncat (from Nmap) plays the server.

## Solution layout

```
prog2200_assign1/
├── prog2200_assign1.sln
├── start-server.ps1                    Optional: starts ncat by hand on port 1234
├── server.ps1                          Receive-only PowerShell test server (cannot reply)
├── prog2200_assign1/                   Windows Forms client (UI only)
│   ├── App.config
│   ├── Program.cs
│   ├── MainForm.cs                     Menu/button handlers, event subscriptions, Invoke
│   ├── MainForm.Designer.cs            Layout: game area, message box, conversation
│   └── Properties/AssemblyInfo.cs
└── prog2200_assign1_library/           Chat library (all chat functionality)
    ├── ChatClient.cs                   Connect / Send / Disconnect, send + receive threads
    ├── ChatEvents.cs                   Event handler delegates, custom EventArgs, ConnectionStatus
    ├── ChatServerLauncher.cs           Opens the ncat server in its own command prompt window
    ├── StateObj.cs                     Per-connection state: socket, buffer, outgoing queue
    └── Properties/AssemblyInfo.cs
```

Both projects are classic Visual Studio .NET Framework 4.8 projects (the same project format as
the course demos). The UI project references the library
(Project → Add → Project Reference), so the library is built into `prog2200_assign1_library.dll`,
copied next to `prog2200_assign1.exe` and loaded by the game.

## Requirements

- Windows 10 or 11 (the .NET Framework 4.8 runtime is built in)
- Visual Studio 2022 with the ".NET desktop development" workload (includes the .NET Framework 4.8 targeting pack)
- Nmap for Windows (https://nmap.org/download), which installs **ncat**
- PowerShell (built into Windows)

## Setup (PowerShell)

### 1. Install the tools

Run in PowerShell. `winget` is built into Windows 10/11. Skip anything you already have.

```powershell
winget install --id Git.Git -e
winget install --id Microsoft.PowerShell -e          # PowerShell 7 (optional - Windows PowerShell 5.1 also works)
winget install --id Insecure.Nmap -e                 # Nmap (with the Zenmap GUI) + ncat

# Visual Studio 2022 Community with the ".NET desktop development" workload (.NET Framework 4.8 support)
winget install --id Microsoft.VisualStudio.2022.Community -e `
    --override "--add Microsoft.VisualStudio.Workload.ManagedDesktop --includeRecommended --passive"
```

Close and reopen PowerShell so the new tools are on your `PATH`, then check them:

```powershell
git --version
ncat --version
```

If `ncat` is not found, add the Nmap folder to `PATH` for the current session:

```powershell
$env:Path += ";${env:ProgramFiles(x86)}\Nmap"
```

### 2. Get the code and build it

```powershell
git clone https://github.com/bakuncwa/prog2200_assign1.git
cd prog2200_assign1
start prog2200_assign1.sln      # opens Visual Studio - then Build -> Build Solution (Ctrl+Shift+B)

# or build from PowerShell with Visual Studio's MSBuild:
& "$env:ProgramFiles\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" prog2200_assign1.sln
```

The game is built to `prog2200_assign1\bin\Debug\prog2200_assign1.exe`, with
`prog2200_assign1_library.dll` beside it.

### 3. Allow scripts (only if `.\start-server.ps1` is blocked)

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass   # current window only
# or permanently for your user:
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
Unblock-File .\start-server.ps1                               # clears the "downloaded from the internet" flag
```

### 4. Configuration

The client connects to `127.0.0.1:1234`. To use a different host or port, change the constants
at the top of `prog2200_assign1/MainForm.cs`:

```csharp
private const string ServerHost = "127.0.0.1";
private const int ServerPort = 1234;
```

and start the server on the same port: `.\start-server.ps1 -Port 5000`.

When ncat first starts, Windows Firewall may ask for access. Allow it on **private** networks.

## Running it

The program follows the steps in the assignment PDF.

1. **Start the client** – open `prog2200_assign1.sln` in Visual Studio and press F5
   (or, once built, run `.\prog2200_assign1\bin\Debug\prog2200_assign1.exe`).
   The *Network Game* window opens with the black game area, *Type your messages here.* and *Conversation*.
2. **Network → Connect** – a separate command prompt window titled
   `Chat Server - ncat -l -p 1234 -t -v -k` opens. This is the server. It shows its
   connections and every message the client sends, and you type the server's replies there:
   ```
   ncat -l -p 1234 -t -v -k
   Ncat: Version 7.95 ( https://nmap.org/ncat )
   Ncat: Listening on [::]:1234
   Ncat: Listening on 0.0.0.0:1234
   Ncat: Connection from 127.0.0.1:52344.
   ```
   If a server is already listening on port 1234 (for example one started with `.\start-server.ps1`),
   it is reused and no new window opens.
3. **Chat** – type in *Type your messages here.* and press **Send** (or Enter). The message appears in the
   server window and as `>>Hello, Server!` in *Conversation*. Type in the server window and press Enter to
   reply; the reply appears in *Conversation* exactly as typed.
4. **Network → Disconnect** closes the connection. The server window keeps running (`-k`), so
   **Network → Connect** reconnects to it.
5. **Game → Exit** or the red **X** closes the game. The server window stays open; close it, or press
   Ctrl+C in it, when you are finished. If the server is closed first, the game shows
   *The server closed the connection.* and keeps running.

### Conversation

| Line | Meaning |
|------|---------|
| `>>Hello, Server! I'm your friendly neighbourhood client!` | Sent by this client |
| `Why, hello, Client! It's nice to meet you!` | Received from the server, exactly as the server typed it |

Problems (no server, not connected, ncat missing) are shown in an **Error** message box.

## How it works

- **Starting the server**: `ChatServerLauncher` checks whether anything is listening on the port. If not, it
  finds `ncat.exe` (on the PATH or in the Nmap install folder), opens it in a new command prompt window and
  waits until it is listening.
- **Connecting** is asynchronous: `ConnectAsync` (`async`/`await`) looks up the IP address and
  connects the socket with a 5-second timeout, so the window never freezes.
- **Receiving** runs on its own thread. It reads bytes into the `StateObj` buffer, decodes them
  back into text and raises `MessageReceived` once per complete line.
- **Sending** runs on its own thread. `Send()` adds the message to a thread-safe
  `BlockingCollection` and returns at once; the send thread writes queued messages in order and
  raises `MessageSent`.
- **Thread safety**: connection state is only changed inside `lock`, so the connection is closed
  exactly once even if the user, the server and a failed send all trigger it at the same moment.
  The form uses `InvokeRequired` / `BeginInvoke` so controls are only touched on the UI thread.

## Verified behaviour (library tested against a live TCP server and ncat)

- No server running → clear "no server is listening on that port" error; an existing server on the port is reused instead of opening a second window
- 500 messages queued in under 1 ms; all arrived at the server in order, with accented characters intact
- A server line split across two TCP packets is displayed as one message, including a character split mid-byte
- A 3,000-character line (longer than the 1,024-byte buffer) arrives as one message
- Server closes first → client reports it cleanly; further sends are rejected with a message
- Client disconnects first → server sees a clean close; disconnecting twice is harmless; reconnect works
- Server killed while the client is sending thousands of messages → exactly one disconnect, no crash

## Troubleshooting

| Problem | Fix |
|---------|-----|
| *The server could not be started - ncat was not found* | Install Nmap (see *Install the tools*), then click Connect again. |
| *The server window opened, but the server did not start listening* | Read the server window. Usually port 1234 is taken: `Get-NetTCPConnection -LocalPort 1234` shows the owner. |
| *Could not connect ... no server is listening on that port* | The server window was closed or stopped. Click Connect again to open a new one. |
| Odd symbols at the start of received text | Start the server by hand without telnet negotiation (`ncat -l -p 1234 -v -k`), then click Connect. |
| Windows Firewall asks about ncat | Allow it on **private** networks. |
| Running on macOS | .NET Framework and Windows Forms only run on Windows. Use a Windows PC or VM. |
