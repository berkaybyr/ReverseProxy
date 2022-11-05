﻿using ReverseProxy.Lib;
using ReverseProxy.Models.Config;
using ReverseProxy.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace ReverseProxy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            _ = RProxy.LoadFromConfig();
            Console.WriteLine("Proxy starting...");
            Console.WriteLine("Write /info to get client connected count for each port");
            Console.WriteLine("Write /stop to stop server");
            Console.WriteLine();
            string str;
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
