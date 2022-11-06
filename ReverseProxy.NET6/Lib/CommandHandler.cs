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

        public static void Handle(string? cmd, List<PortForwarder> forwarders)
        {
            if (cmd is null) return;
            if (cmd == "/info")
            {
                Info(forwarders);
            }
            else if (cmd == "/help")
            {
                Help();
            }
            else if(cmd?.StartsWith("/list") == true)
            {
                var data = cmd.Split(" ");
                if (data.Length < 2)
                {
                    List(forwarders);
                }
                else
                {
                    var forwarderNo = int.Parse(data[1]);
                    List(forwarders, forwarderNo);
                }
            }
        }
        private static void Info(List<PortForwarder> list)
        {
            Console.WriteLine();
            EasLogConsole.Info("Current forwarder information;");
            var count = 1;
            list.ForEach(x =>
            {
                var connected = x.Clients.Count;
                var host = x.Config.Host.IpAddress;
                var hostPort = x.Config.Host.Port;
                EasLogConsole.Info(count + ". Host: {0}:{1} ForwardTo: {2}:{3} ConnectedClient: {4}".FormatString(host,hostPort, x.Config.Forward.IpAddress, x.Config.Forward.Port ,connected));
                count++;
            });
        }
        private static void Help()
        {
            EasLogConsole.Info("Write /list <forwarder no> to get client list for a specific forwarder");
            EasLogConsole.Info("Write /list to get client list for all forwarders");
            EasLogConsole.Info("Write /help to get command list");
            EasLogConsole.Info("Write /info to get client connected count for each port");
            EasLogConsole.Info("Write /stop to stop server");
            Console.WriteLine();
        }
        private static void List(List<PortForwarder> list, int forwarderNo)
        {
            if (forwarderNo > 0 && forwarderNo > list.Count)
            {
                EasLogConsole.Error("Invalid forwarder no");
                return;
            }
            var selected = list[forwarderNo - 1];
            EasLogConsole.Info($"[{selected.Config.Host.IpAddress}:{selected.Config.Host.Port}] Connected clients;");
            foreach (var item in selected.Clients)
            {
                EasLogConsole.Info("[{0}] [{1}]".FormatString(item.Key, item.Value.SourceClient.Client.RemoteEndPoint?.ToString()));
            }
        }
        private static void List(List<PortForwarder> list)
        {
            
            foreach(var forewarder in list)
            {
                Console.WriteLine();
                EasLogConsole.Info($"[{forewarder.Config.Host.IpAddress}:{forewarder.Config.Host.Port}] Connected clients;");
                foreach (var item in forewarder.Clients)
                {
                    EasLogConsole.Info("[{0}] [{1}]".FormatString(item.Key, item.Value.SourceClient.Client.RemoteEndPoint?.ToString()));
                }
            }
        }
    }
}
