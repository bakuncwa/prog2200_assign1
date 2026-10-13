using System;
using System.Windows.Forms;

namespace prog2200_assign1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show any unexpected error in a message box instead of letting the program close silently.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (sender, e) => ShowUnexpectedError(e.Exception);
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, e) => ShowUnexpectedError(e.ExceptionObject as Exception);

            Application.Run(new MainForm());

#if DEBUG
            // TEMPORARY DIAGNOSTIC: confirms the main window finished normally. Remove once the
            // early-exit problem is found.
            MessageBox.Show("Application.Run returned - the main window has closed.", "DEBUG - program ending");
#endif
        }

        private static void ShowUnexpectedError(Exception exception)
        {
            MessageBox.Show("An unexpected error occurred:" + Environment.NewLine + Environment.NewLine + exception,
                "Network Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
