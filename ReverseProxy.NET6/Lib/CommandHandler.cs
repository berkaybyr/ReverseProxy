using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.NET6.Lib
{
    public static class CommandHandler
    {
        private static readonly EasLog logger = IEasLog.CreateLogger("CommandHandler");
        public static void Info(List<PortForwarder> list)
        {
            Console.WriteLine();
            EasLogConsole.Info("Current connected client counts;");
            list.ForEach(x =>
            {
                var connected = x.Clients.Count;
                var host = x.Config.Host.IpAddress;
                var hostPort = x.Config.Host.Port;
                EasLogConsole.Info("HOST: {0}:{1} COUNT: {2}".FormatString(host,hostPort,connected));
            });
        }
        public static void Help()
        {
            Console.WriteLine();
            EasLogConsole.Info("Write /help to get command list");
            EasLogConsole.Info("Write /info to get client connected count for each port");
            EasLogConsole.Info("Write /stop to stop server");
            Console.WriteLine();
        }
    }
}
