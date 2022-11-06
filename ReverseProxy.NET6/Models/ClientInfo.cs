using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace ReverseProxy.NET6.Models
{
    public class ClientInfo
    {
        public Guid Id { get; set; }
        public DateTime ConnectedAt { get; private set; } = DateTime.Now;
        public TcpClient DestClient { get; set; }
        public TcpClient SourceClient { get; set; }
        public CopyStream SourceToDest { get; set; }
        public CopyStream DestToSource { get; set; }
        public string? ClientRemoteIp
        {
            get
            {
                if (SourceClient != null)
                {
                    var ip = SourceClient.Client.RemoteEndPoint?.ToString()?.Split(":")[0];
                    return ip;
                }
                return null;
            }
        }
        public string? DestinationIp
        {
            get
            {
                if (DestClient != null)
                {
                    var ip = DestClient.Client.RemoteEndPoint?.ToString()?.Split(":")[0];
                    return ip;
                }
                return null;
            }
        }
    }
}