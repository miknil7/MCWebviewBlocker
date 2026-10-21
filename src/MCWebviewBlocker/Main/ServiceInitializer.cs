using System;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Collections.Generic;

namespace MCWebviewBlocker
{
    public partial class ServiceInitializer : ServiceBase
    {
        private List<FunctionBase> _functions = new List<FunctionBase>();

        public ServiceInitializer()
        {
            InitializeComponent();
            SetServiceDescription();

            _functions.Add(new MCWebviewBlocker());

            return;
        }

        private void SetServiceDescription()
        {
            var description = "Blocks MC Webview pop-up notifications displayed in the bottom-right corner.";

            if (CultureInfo.InstalledUICulture.TwoLetterISOLanguageName == "ja")
            {
                description = "右下に表示されるMC Webviewポップアップ表示をブロックします。";
            }

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(
                    $"SYSTEM\\CurrentControlSet\\Services\\{ServiceName}", true))
                {
                    key?.SetValue("Description", description, RegistryValueKind.String);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // admin??
            }
        }

        public void StartDebug(string[] args) => OnStart(args);
        public void StopDebug() => OnStop();

        protected override void OnStart(string[] args)
        {
            if (0 == _functions.Count())
            {
                Console.WriteLine($"===== The function to be executed has not been set. =====");
                throw new Exception("The function to be executed has not been set.");
            }

            foreach (var function in _functions)
            {
                function.Start();
            }

            return;
        }

        protected override void OnStop()
        {
            foreach(var function in _functions)
            {
                function.Stop();
            }

            foreach(var function in _functions)
            {
                function.Join();
            }

            return;
        }
    }
}
