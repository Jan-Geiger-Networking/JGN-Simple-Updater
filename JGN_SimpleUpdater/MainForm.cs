using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Reflection;

namespace JGN_SimpleUpdater
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer downloadProgressTimer;
        private int totalPackages = 0;
        private int finishedPackages = 0;
        private bool progressBarInitialized = false;
        private List<string> updatedApps = new List<string>();
        private List<string> failedApps = new List<string>();
        private Process wingetProcess = null;
        private static readonly HttpClient httpClient = new HttpClient();
        private const string GITHUB_REPO_URL = "https://api.github.com/repos/Jan-Geiger-Networking/JGN-Simple-Updater/releases/latest";

        // GitHub Release-Klasse für JSON-Deserialisierung
        private class GitHubRelease
        {
            public string tag_name { get; set; } = "";
            public string html_url { get; set; } = "";
            public string body { get; set; } = "";
        }

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadConsoleOutputCharacter(IntPtr hConsoleOutput, [Out] StringBuilder lpCharacter, uint nLength, COORD dwReadCoord, out uint lpNumberOfCharsRead);
        [StructLayout(LayoutKind.Sequential)]
        public struct COORD { public short X; public short Y; }
        const int STD_OUTPUT_HANDLE = -11;
        const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "JGN Simple Updater (WinGet)";
            this.MinimumSize = this.Size;
            downloadProgressTimer = new System.Windows.Forms.Timer();
            downloadProgressTimer.Interval = 500;
            
            // Event für das Schließen des Fensters hinzufügen
            this.FormClosing += MainForm_FormClosing;
            
            // winget-Installationsstatus prüfen
            CheckWingetInstallation();
            
            // Versionscheck beim Starten (asynchron)
            _ = CheckForUpdatesAsync();
        }

        private async void btnStartUpdate_Click(object sender, EventArgs e)
        {
            // Zusätzliche Prüfung, ob winget verfügbar ist
            if (!WingetChecker.IsWingetInstalled() || !WingetChecker.IsWingetFunctional())
            {
                MessageBox.Show(
                    "winget ist nicht verfügbar oder funktioniert nicht.\n\n" +
                    "Bitte installieren Sie winget neu und starten Sie das Programm neu.",
                    "winget nicht verfügbar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
            btnStartUpdate.Enabled = false;
            btnExit.Enabled = false;
            
            // Listen für Updates zurücksetzen
            updatedApps.Clear();
            failedApps.Clear();

            await Task.Run(() => RunWingetUpdate());

            progressBar1.Value = progressBar1.Maximum;
            btnStartUpdate.Enabled = true;
            btnExit.Enabled = true;
            
            // Abschlussfenster anzeigen
            ShowCompletionDialog();
        }

        private void RunWingetUpdate()
        {
            downloadProgressTimer.Start();
            totalPackages = 0;
            finishedPackages = 0;
            progressBarInitialized = false;
            textBox1.Clear();
            
            var psi = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "upgrade --all --silent --accept-source-agreements --accept-package-agreements",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8
            };

            wingetProcess = new Process();
            wingetProcess.StartInfo = psi;
            wingetProcess.Start();

            var output = wingetProcess.StandardOutput;
            int progress = 0;
            bool updateStarted = false;
            string currentPackage = "-";

            while (!output.EndOfStream)
            {
                var line = output.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                // Spinner-Zeilen filtern
                string trimmed = line.Trim();
                if (trimmed.Length > 0 && trimmed.All(c => c == '-' || c == '|' || c == '/' || c == '\\'))
                {
                    continue; // Spinner-Zeile überspringen
                }
                
                // Zeile zur TextBox hinzufügen
                BeginInvoke((System.Windows.Forms.MethodInvoker)(() =>
                {
                    textBox1.AppendText(line + Environment.NewLine);
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.ScrollToCaret();
                }));
                
                ProcessWingetLineForProgress(line, ref progress, ref updateStarted, ref currentPackage);
            }
            downloadProgressTimer.Stop();
            wingetProcess.WaitForExit();
            wingetProcess = null; // Prozess-Referenz zurücksetzen
        }

        private void ProcessWingetLineForProgress(string line, ref int progress, ref bool updateStarted, ref string currentPackage)
        {
            // Gesamtanzahl Pakete erkennen
            if (line.Contains("Found ") && line.Contains("/"))
            {
                // Beispiel: (2/3) Found ...
                int start = line.IndexOf('(');
                int slash = line.IndexOf('/', start);
                int end = line.IndexOf(')', slash);
                if (start >= 0 && slash > start && end > slash)
                {
                    if (int.TryParse(line.Substring(slash + 1, end - slash - 1), out int total))
                    {
                        totalPackages = total;
                        if (!progressBarInitialized)
                        {
                            progressBar1.Maximum = total;
                            progressBar1.Value = 0;
                            progressBarInitialized = true;
                        }
                    }
                }
                // Paketname extrahieren
                int idx = line.IndexOf("Found ");
                string rest = line.Substring(idx + 6);
                int bracket = rest.IndexOf('[');
                if (bracket > 0)
                    rest = rest.Substring(0, bracket).Trim();
                currentPackage = rest;
                labelPackageStatus.Text = $"Paketstatus: {currentPackage} - Wird verarbeitet...";
            }
            // Installationsfortschritt (z.B. "Installing" oder "Downloading")
            else if (line.Contains("Downloading") || line.Contains("Installing") || 
                     line.Contains("Starting package install") || line.Contains("Verifying") ||
                     line.Contains("Installing dependencies") || line.Contains("Successfully verified"))
            {
                string statusText = line.Trim();
                labelPackageStatus.Text = $"Paketstatus: {currentPackage} - {statusText}";
            }
            // Abschluss eines Pakets
            else if (line.Contains("Successfully installed") || line.Contains("No updates available"))
            {
                labelPackageStatus.Text = $"Paketstatus: {currentPackage} - Fertig";
                finishedPackages++;
                if (totalPackages > 0)
                {
                    progressBar1.Value = Math.Min(finishedPackages, totalPackages);
                }
                
                // Erfolgreich aktualisierte App zur Liste hinzufügen
                if (!string.IsNullOrEmpty(currentPackage) && currentPackage != "-")
                {
                    if (!updatedApps.Contains(currentPackage))
                    {
                        updatedApps.Add(currentPackage);
                    }
                }
            }
            // Fehler
            else if (line.Contains("Fehler") || line.Contains("error") || line.Contains("failed"))
            {
                labelPackageStatus.Text = $"Paketstatus: {currentPackage} - Fehler";
                finishedPackages++;
                if (totalPackages > 0)
                {
                    progressBar1.Value = Math.Min(finishedPackages, totalPackages);
                }
                
                // Fehlgeschlagene App zur Liste hinzufügen
                if (!string.IsNullOrEmpty(currentPackage) && currentPackage != "-")
                {
                    if (!failedApps.Contains(currentPackage))
                    {
                        failedApps.Add(currentPackage);
                    }
                }
            }
            // Andere wichtige Status-Informationen
            else if (line.Contains("upgrades available") || line.Contains("Found ") || 
                     line.Contains("Downloading") || line.Contains("Installing"))
            {
                string statusText = line.Trim();
                if (statusText.Length > 50)
                {
                    statusText = statusText.Substring(0, 47) + "...";
                }
                labelPackageStatus.Text = $"Status: {statusText}";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        private void ShowCompletionDialog()
        {
            string message = "Updates abgeschlossen!\n\n";
            
            if (updatedApps.Count > 0)
            {
                message += "Erfolgreich aktualisierte Apps:\n";
                foreach (string app in updatedApps)
                {
                    message += $"• {app}\n";
                }
                message += "\n";
            }
            
            if (failedApps.Count > 0)
            {
                message += "Fehlgeschlagene Updates:\n";
                foreach (string app in failedApps)
                {
                    message += $"• {app}\n";
                }
                message += "\n";
            }
            
            if (updatedApps.Count == 0 && failedApps.Count == 0)
            {
                message += "Keine Updates gefunden oder alle Apps sind bereits aktuell.";
            }
            
            MessageBox.Show(message, "Updates abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Wenn ein winget-Prozess läuft, diesen beenden
            if (wingetProcess != null && !wingetProcess.HasExited)
            {
                try
                {
                    wingetProcess.Kill();
                    wingetProcess.WaitForExit(3000); // Maximal 3 Sekunden warten
                }
                catch (Exception ex)
                {
                    // Fehler beim Beenden des Prozesses ignorieren
                    Debug.WriteLine($"Fehler beim Beenden des winget-Prozesses: {ex.Message}");
                }
                finally
                {
                    wingetProcess = null;
                }
            }
            
            // Timer stoppen falls er läuft
            if (downloadProgressTimer != null && downloadProgressTimer.Enabled)
            {
                downloadProgressTimer.Stop();
            }
        }

        private void CheckWingetInstallation()
        {
            // Zuerst winget -v in der TextBox ausführen, damit der Benutzer sehen kann, was passiert
            textBox1.Text = "Prüfe winget Installation...\r\n";
            textBox1.AppendText("Führe 'winget -v' aus...\r\n\r\n");
            
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "winget",
                        Arguments = "-v",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit(5000);

                var output = process.StandardOutput.ReadToEnd().Trim();
                var error = process.StandardError.ReadToEnd().Trim();

                textBox1.AppendText($"Exit Code: {process.ExitCode}\r\n");
                textBox1.AppendText($"Standard Output: '{output}'\r\n");
                textBox1.AppendText($"Standard Error: '{error}'\r\n\r\n");

                if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output) && output.StartsWith("v") && output.Any(char.IsDigit))
                {
                    textBox1.AppendText("✅ winget ist installiert und funktioniert!\r\n");
                    textBox1.AppendText($"Version: {output}\r\n\r\n");
                    textBox1.AppendText("Tool ist einsatzbereit!\r\n\r\n");
                }
                else
                {
                    textBox1.AppendText("❌ winget ist NICHT installiert oder funktioniert nicht.\r\n");
                    textBox1.AppendText("Zeige Installationsdialog...\r\n\r\n");
                    
                    // winget ist nicht installiert - Dialog anzeigen
                    var wingetForm = new WingetInstallForm();
                    var result = wingetForm.ShowDialog();
                    
                    if (!wingetForm.WingetWillBeInstalled)
                    {
                        // Benutzer hat abgebrochen - Tool deaktivieren
                        textBox1.AppendText("Benutzer hat Installation abgebrochen. Tool wird deaktiviert.\r\n");
                        DisableApplication();
                    }
                    else
                    {
                        textBox1.AppendText("Benutzer wird winget installieren. Bitte starten Sie das Programm neu.\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                textBox1.AppendText($"❌ Fehler beim Ausführen von winget -v:\r\n{ex.Message}\r\n\r\n");
                textBox1.AppendText("Zeige Installationsdialog...\r\n\r\n");
                
                // winget ist nicht installiert - Dialog anzeigen
                var wingetForm = new WingetInstallForm();
                var result = wingetForm.ShowDialog();
                
                if (!wingetForm.WingetWillBeInstalled)
                {
                    // Benutzer hat abgebrochen - Tool deaktivieren
                    textBox1.AppendText("Benutzer hat Installation abgebrochen. Tool wird deaktiviert.\r\n");
                    DisableApplication();
                }
                else
                {
                    textBox1.AppendText("Benutzer wird winget installieren. Bitte starten Sie das Programm neu.\r\n");
                }
            }
        }

        private void DisableApplication()
        {
            // Alle Buttons deaktivieren
            btnStartUpdate.Enabled = false;
            btnAbout.Enabled = false;
            
            // TextBox und ProgressBar deaktivieren
            textBox1.Enabled = false;
            progressBar1.Enabled = false;
            
            // Status-Label aktualisieren
            labelPackageStatus.Text = "Tool deaktiviert - winget erforderlich";
            
            // Fenster-Titel aktualisieren
            this.Text = "JGN Simple Updater (WinGet) - DEAKTIVIERT";
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                // Aktuelle Version aus Assembly abrufen
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (currentVersion == null) return;

                // GitHub API abfragen
                httpClient.DefaultRequestHeaders.Add("User-Agent", "JGN_SimpleUpdater");
                var response = await httpClient.GetStringAsync(GITHUB_REPO_URL);
                var release = JsonSerializer.Deserialize<GitHubRelease>(response);

                if (release != null && !string.IsNullOrEmpty(release.tag_name))
                {
                    // Version aus Tag parsen (z.B. "v1.0.0.2" -> Version 1.0.0.2)
                    var versionString = release.tag_name.TrimStart('v');
                    if (Version.TryParse(versionString, out var latestVersion))
                    {
                        if (latestVersion > currentVersion)
                        {
                            // Neue Version verfügbar - Dialog anzeigen
                            BeginInvoke(new Action(() => ShowUpdateAvailableDialog(release)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fehler beim Versionscheck ignorieren (nicht kritisch)
                Debug.WriteLine($"Fehler beim Versionscheck: {ex.Message}");
            }
        }

        private void ShowUpdateAvailableDialog(GitHubRelease release)
        {
            var message = $"Eine neue Version ist verfügbar!\n\n" +
                         $"Aktuelle Version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                         $"Neue Version: {release.tag_name}\n\n" +
                         $"Möchten Sie die neue Version herunterladen?";

            var result = MessageBox.Show(message, "Update verfügbar", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes)
            {
                // Download-Link öffnen
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = release.html_url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Öffnen des Download-Links:\n{ex.Message}", 
                        "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
