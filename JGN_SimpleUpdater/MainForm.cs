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

namespace JGN_SimpleUpdater
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Timer downloadProgressTimer;
        private int totalPackages = 0;
        private int finishedPackages = 0;
        private bool progressBarInitialized = false;

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
        }

        private async void btnStartUpdate_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;
            btnStartUpdate.Enabled = false;
            btnExit.Enabled = false;

            await Task.Run(() => RunWingetUpdate());

            progressBar1.Value = progressBar1.Maximum;
            btnStartUpdate.Enabled = true;
            btnExit.Enabled = true;
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

            using var process = new Process();
            process.StartInfo = psi;
            process.Start();

            var output = process.StandardOutput;
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
                BeginInvoke((MethodInvoker)(() =>
                {
                    textBox1.AppendText(line + Environment.NewLine);
                    textBox1.SelectionStart = textBox1.Text.Length;
                    textBox1.ScrollToCaret();
                }));
                
                ProcessWingetLineForProgress(line, ref progress, ref updateStarted, ref currentPackage);
            }
            downloadProgressTimer.Stop();
            process.WaitForExit();
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
                labelPackageStatus.Text = $"Paketstatus: {currentPackage}";
            }
            // Installationsfortschritt (z.B. "Installing" oder "Downloading")
            else if (line.Contains("Downloading") || line.Contains("Installing"))
            {
                labelPackageStatus.Text = $"Paketstatus: {currentPackage} - {line.Trim()}";
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
    }
}
