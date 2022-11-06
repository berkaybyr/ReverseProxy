using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ReverseProxy.NET6.Lib
{
    public class PortForwarder
    {
        private static readonly EasLog logger = IEasLog.CreateLogger("PortForwarder");
        public Guid Id { get; private set; }
        private bool m_started = false;
        public TcpListener Server { get; set; }
        public Task ServerTask { get; private set; }
        public ConcurrentDictionary<Guid, ClientInfo> Clients { get; set; }
        public ReverseProxyConfig Config { get; private set; }
        public PortForwarder(ReverseProxyConfig config)
        {
            Config = config;
            Id = Guid.NewGuid();
            Clients = new ConcurrentDictionary<Guid, ClientInfo>();
            Server = new TcpListener(IPAddress.Parse(config.Host.IpAddress), config.Host.Port);
        }
        public void StartServer()
        {
            if (m_started)
            {
                return;
            }

            m_started = true;
            ServerTask = Task.Factory.StartNew(RunServer);
        }

        private void RunServer()
        {
            Server.Start();
            Server.BeginAcceptTcpClient(EndAcceptSocketTcpClient, null);
            logger.Info("Listening from {0}:{1} forwarded to {2}:{3}".FormatString(Config.Host.IpAddress, Config.Host.Port, Config.Forward.IpAddress, Config.Forward.Port));
        }

        private void EndAcceptSocketTcpClient(IAsyncResult ar)
        {
            var client = Server.EndAcceptTcpClient(ar);
            var remoteIp = client.Client.RemoteEndPoint?.ToString();
            if (!IsAllowedByFilter(client.Client.RemoteEndPoint?.ToString()))
            {
                logger.Warn("Client {0} is not allowed to connect to {1}:{2} due to connection filter".FormatString(remoteIp, Config.Host.IpAddress, Config.Host.Port));
                CloseBeforeCreate(client);
                Server.BeginAcceptTcpClient(EndAcceptSocketTcpClient, null);
                return;
            }
            if (!IsAllowedByConnectionLimitPerIp(client.Client.RemoteEndPoint?.ToString(), out var count))
            {
                logger.Warn("Client {0} has reached the connection limit of {1}/{2} to {3}:{4}".FormatString(remoteIp, count, Config.ConnectionLimitPerIp, Config.Host.IpAddress, Config.Host.Port));
                CloseBeforeCreate(client);
                Server.BeginAcceptTcpClient(EndAcceptSocketTcpClient, null);
                return;
            }
            if (!IsAllowedByPortConnectionRequirement(client.Client.RemoteEndPoint?.ToString()))
            {
                logger.Warn("Client {0} connection now allowed to {3}:{4} due to port connection requirement".FormatString(remoteIp, count, Config.ConnectionLimitPerIp, Config.Host.IpAddress, Config.Host.Port));
                CloseBeforeCreate(client);
                Server.BeginAcceptTcpClient(EndAcceptSocketTcpClient, null);
                return;
            }

            var id = Guid.NewGuid();
            var info = new ClientInfo
            {
                Id = id,
                SourceClient = client,
            };
            Clients[id] = info;
            logger.Info(id, remoteIp ?? "-", "Client connected to {1}:{2}".FormatString(remoteIp, Config.Host.IpAddress, Config.Host.Port));
            Task.Factory.StartNew(() => HandleClient(info));
            Server.BeginAcceptTcpClient(EndAcceptSocketTcpClient, null);
        }

        private void HandleClient(ClientInfo info)
        {
            var destClient = new TcpClient();
            destClient.ReceiveTimeout = Config.ReceiveTimeout;
            destClient.SendTimeout = Config.SendTimeout;
            info.DestClient = destClient;
            logger.Info(info.Id, info.SourceClient.Client.RemoteEndPoint?.ToString() ?? "-", "Connecting to {0}:{1}".FormatString(Config.Forward.IpAddress, Config.Forward.Port));
            destClient.BeginConnect(Config.Forward.IpAddress, Config.Forward.Port, EndConnectWriter, info);
        }

        private void EndConnectWriter(IAsyncResult ar)
        {
            var info = (ClientInfo)ar.AsyncState;
            info.DestClient.EndConnect(ar);
            info.SourceToDest = new CopyStream(info.SourceClient, info.DestClient, () => Close(info.Id));
            info.DestToSource = new CopyStream(info.DestClient, info.SourceClient, () => Close(info.Id));
        }
        private void CloseBeforeCreate(TcpClient client)
        {
            
            try
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error closing client {0}".FormatString(client.Client.RemoteEndPoint?.ToString() ?? ""));
            }
        }
        private void Close(Guid id)
        {
            if (Clients.TryRemove(id, out ClientInfo info))
            {
                try
                {
                    if (info.SourceClient.Connected)
                    {
                        info.SourceClient.Close();
                    }
                    if (info.DestClient.Connected)
                    {
                        info.DestClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error closing client {0}".FormatString(id));
                }
            }
        }
        private bool IsAllowedByFilter(string? ip)
        {
            try
            {
                if (ip == null || ip == string.Empty) return false;
                if (Config.FilterConnection?.IpAddresses != null && Config.FilterConnection.IpAddresses?.Count > 0)
                {
                    if (!Config.FilterConnection.IpAddresses.Contains(ip ?? ""))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error checking connection filtering Exception: {0}".FormatString(ex.JsonSerialize()));
                return false;
            }
        }
        private bool IsAllowedByConnectionLimitPerIp(string? ip, out int count)
        {
            try
            {
                count = 0;
                var realIp = ip?.Split(":")[0];
                if (realIp == null || realIp == string.Empty) return false;
                count = Clients.Values.Where(x => x.ClientRemoteIp?.Contains(realIp) == true).Count();
                if (count >= Config.ConnectionLimitPerIp)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error checking connection limit per ip Exception: {0}".FormatString(ex.JsonSerialize()));
                count = 0;
                return false;
            }
        }
        private bool IsAllowedByPortConnectionRequirement(string? ip)
        {
            try
            {
                var realIp = ip.ToIpAddressString();
                if (ip is null) return false;
                if (Config.RequireConnectionToPort?.Ports is null || Config.RequireConnectionToPort?.Ports?.Count == 0) return true;
                foreach (var port in Config.RequireConnectionToPort.Ports)
                {
                    var forwarder = RProxy.ForwarderMap.FirstOrDefault(x => x.Key.Port == port);
                    if (!forwarder.Value.Clients.Any(x => x.Value.ClientRemoteIp == realIp))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error checking port connection requirement Exception: {0}".FormatString(ex.JsonSerialize()));
                return false;
            }
        }
    }
}