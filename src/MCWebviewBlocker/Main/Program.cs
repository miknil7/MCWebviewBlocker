using System;
using System.ServiceProcess;
using System.Threading;

namespace MCWebviewBlocker
{
    internal static class Program
    {
        static void Main()
        {
            var service = new ServiceInitializer();

            if (true == System.Environment.UserInteractive)
            {
                var wh = new EventWaitHandle(false, EventResetMode.AutoReset, "StopService");
                service.StartDebug(null);
                wh.WaitOne();
                service.StopDebug();
            }
            else
            {
                ServiceBase.Run(service);
            }

            return;
        }
    }
}
