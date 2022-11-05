
using ReverseProxy.NET6.Lib;
using System;
using System.Collections.Generic;
using System.Net;

namespace ReverseProxy.NET6
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlHandler.Read();
            _ = RProxy.LoadFromConfig();
            Console.WriteLine("Proxy starting...");
            Console.WriteLine("Write /info to get client connected count for each port");
            Console.WriteLine("Write /stop to stop server");
            Console.WriteLine();
            string? str;
            do
            {
                str = Console.ReadLine();
                if (str == "/info")
                {

                }
            }
            while (str != "/stop");

        }
    }
}
