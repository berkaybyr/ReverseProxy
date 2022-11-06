
using ReverseProxy.NET6.Lib;
using System;
using System.Collections.Generic;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace ReverseProxy.NET6
{
    internal class Program
    {
        private static readonly EasLog logger = IEasLog.CreateLogger("Main");
        
        private static void Main(string[] args)
        {
            IEasLog.LoadConfig(new EasLogConfiguration
            {
                LogFileName = "RProxy",
                AddRequestUrlToStart = false,
                ConsoleAppender = true,
                ExceptionHideSensitiveInfo = false,
                IsDebug = true,
                TraceLogging = true,
                WebInfoLogging = false,
            });
            
            var proxy = RProxy.LoadFromConfig();
            var forwarders = new List<PortForwarder>();
            foreach (var item in proxy)
            {
                foreach (var forwarder in item.Forwarders)
                {
                    forwarders.Add(forwarder);
                }
            }
            Thread.Sleep(1000);
            string? str;
            do
            {
                str = Console.ReadLine();
                CommandHandler.Handle(str,forwarders);
            }
            while (str != "/stop");
        }
    }
}
