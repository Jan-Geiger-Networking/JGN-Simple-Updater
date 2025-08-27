using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace JGN_SimpleUpdater
{
    public partial class WingetInstallForm : Form
    {
        public bool WingetWillBeInstalled { get; private set; } = false;

        public WingetInstallForm()
        {
            InitializeComponent();
        }

        private void btnInstallWinget_Click(object sender, EventArgs e)
        {
            WingetWillBeInstalled = true;
            
            try
            {
                // winget direkt über PowerShell Gallery installieren (Microsoft-Offiziell)
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command \"$progressPreference = 'silentlyContinue'; Write-Host 'Installing WinGet PowerShell module from PSGallery...'; Install-PackageProvider -Name NuGet -Force | Out-Null; Install-Module -Name Microsoft.WinGet.Client -Force -Repository PSGallery | Out-Null; Write-Host 'Using Repair-WinGetPackageManager cmdlet to bootstrap WinGet...'; Repair-WinGetPackageManager -AllUsers; Write-Host 'Done.'\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = false,
                        Verb = "runas" // Als Administrator ausführen
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show(
                        "winget wurde erfolgreich installiert! Bitte starten Sie das Programm neu.",
                        "Installation erfolgreich",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "winget konnte nicht installiert werden. Bitte versuchen Sie es manuell über die GitHub-Seite.",
                        "Installation fehlgeschlagen",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler bei der Installation:\n{ex.Message}\n\nBitte versuchen Sie es manuell über die GitHub-Seite.",
                    "Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            WingetWillBeInstalled = false;
            this.Close();
        }

        private void btnManualInstall_Click(object sender, EventArgs e)
        {
            WingetWillBeInstalled = true;
            
            try
            {
                // GitHub Release-Seite für winget öffnen
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/microsoft/winget-cli/releases",
                    UseShellExecute = true
                });
                
                MessageBox.Show(
                    "Die GitHub-Seite für winget wurde geöffnet. Bitte laden Sie die neueste Version herunter und installieren Sie sie manuell. Starten Sie danach das Programm neu.",
                    "Manuelle winget Installation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler beim Öffnen der GitHub-Seite:\n{ex.Message}\n\nBitte besuchen Sie https://github.com/microsoft/winget-cli/releases manuell.",
                    "Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            
            this.Close();
        }
    }
}
