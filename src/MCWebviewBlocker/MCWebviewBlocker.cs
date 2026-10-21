using System;
using System.Diagnostics;
using System.Management;

namespace MCWebviewBlocker
{
    public class MCWebviewBlocker : FunctionBase
    {
        private const int INVALID = -1;
        private const int SERVICE_INTERVAL = 500;   // msec
        private const string MC_VIEW_KEY = "mc-web-view";
        private const string MC_TARGET_VALUE = "/alert";

        public MCWebviewBlocker() : base(SERVICE_INTERVAL)
        {
            Function += () =>
            {
                webviewKill();

                return;
            };

            return;
        }

        /// <summary>
        /// Closing the WebView
        /// </summary>
        /// <note>
        /// The service must be run with administrator privileges.
        /// </note>
        private static void webviewKill()
        {
            foreach (var process in Process.GetProcessesByName(MC_VIEW_KEY))
            {
                try
                {
                    var pid = GetPIDfromProcess(process.ProcessName, MC_TARGET_VALUE);

                    if (process.Id == pid)
                    {
                        Console.WriteLine($"===== kill MCWebview ({DateTime.Now}) =====");
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    // admin??????????
                    Console.WriteLine($"===== {Environment.NewLine}{ex.Message}{Environment.NewLine} =====");
                }
            }

            return;
        }

        public static int GetPIDfromProcess(string ProcessName, string TargetCommandLine = "")
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                return INVALID;
            }

            try
            {
                var query = $"SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = '{ProcessName}.exe'";

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    using (var results = searcher.Get())
                    {
                        foreach (var mo in results)
                        {
                            var pid = mo["ProcessId"];

                            if (string.IsNullOrEmpty(TargetCommandLine))
                            {
                                return Convert.ToInt32(pid);
                            }

                            var commandLine = mo["CommandLine"];

                            if (null != commandLine)
                            {
                                if (commandLine.ToString().IndexOf(TargetCommandLine, StringComparison.Ordinal) >= 0)
                                {
                                    return Convert.ToInt32(pid);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"===== {Environment.NewLine}{ex.Message}{Environment.NewLine} =====");
            }

            return INVALID;
        }

    }   // class
}
