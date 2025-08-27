using System;
using System.Diagnostics;
using System.IO;

namespace JGN_SimpleUpdater
{
    public static class WingetChecker
    {
        /// <summary>
        /// Prüft, ob winget auf dem System installiert ist
        /// </summary>
        /// <returns>true, wenn winget verfügbar ist, sonst false</returns>
        public static bool IsWingetInstalled()
        {
            try
            {
                // Einfach: winget -v ausführen
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
                process.WaitForExit(5000); // Maximal 5 Sekunden warten

                if (process.ExitCode == 0)
                {
                    var output = process.StandardOutput.ReadToEnd().Trim();
                    // Wenn Ausgabe mit "v" beginnt und Zahlen enthält, ist winget installiert
                    var result = !string.IsNullOrWhiteSpace(output) && output.StartsWith("v") && output.Any(char.IsDigit);
                    System.Diagnostics.Debug.WriteLine($"WingetChecker: winget -v erfolgreich - Output: '{output}', Result: {result}");
                    return result;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"WingetChecker: winget -v fehlgeschlagen - ExitCode: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WingetChecker: Exception: {ex.Message}");
            }

            System.Diagnostics.Debug.WriteLine("WingetChecker: winget nicht gefunden");
            return false;
        }

        /// <summary>
        /// Prüft, ob winget funktionsfähig ist, indem ein einfacher Befehl ausgeführt wird
        /// </summary>
        /// <returns>true, wenn winget funktioniert, sonst false</returns>
        public static bool IsWingetFunctional()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "winget",
                        Arguments = "list",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit(10000); // Maximal 10 Sekunden warten

                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
