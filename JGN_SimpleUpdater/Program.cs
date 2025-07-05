using System.Security.Principal;
using System.Diagnostics;

namespace JGN_SimpleUpdater
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsAdministrator())
            {
                var mainModule = Process.GetCurrentProcess().MainModule;
                if (mainModule == null || string.IsNullOrEmpty(mainModule.FileName))
                {
                    MessageBox.Show("Konnte den Pfad zur EXE nicht ermitteln!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var exeName = mainModule.FileName;
                var startInfo = new ProcessStartInfo(exeName)
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch
                {
                    MessageBox.Show("Die Anwendung muss als Administrator gestartet werden!", "Administratorrechte benötigt", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }

        static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}