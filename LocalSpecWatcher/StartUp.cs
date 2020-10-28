using LocalSpecWatcher.Service;
using System;
using Topshelf;

namespace LocalSpecWatcher
{
    public class StartUp
    {
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<LoggingService>();
                x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
                x.SetServiceName("LocalSpecWatcher");
                x.StartAutomatically();
            });
        }
    }
}
