﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace ReverseProxy.NET6.Models
{
    public class ClientInfo
    {
        public Guid Id { get; set; }
        public  DateTime ConnectedAt { get; private set; } = DateTime.Now;
        public TcpClient DestClient { get; set; }
        public TcpClient SourceClient { get; set; }
        public CopyStream SourceToDest { get; set; }
        public CopyStream DestToSource { get; set; }
    }
}