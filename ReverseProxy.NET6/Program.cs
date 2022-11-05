
using ReverseProxy.NET6.Lib;
using System;
using System.Collections.Generic;
using System.Net;

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

            Thread.Sleep(1000);
            CommandHandler.Help();

            string? str;
            do
            {
                str = Console.ReadLine();
                if (str == "/info")
                {
                    var list = new List<PortForwarder>();
                    foreach(var item in proxy)
                    {
                        foreach(var forwarder in item.Forwarders)
                        {
                            list.Add(forwarder);
                        }
                    }
                    CommandHandler.Info(list);
                }
            }
            while (str != "/stop");

        }
    }
}
