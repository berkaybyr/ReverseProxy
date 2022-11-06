using System.Configuration;

namespace ReverseProxy.NET6.Lib
{
    public class RProxy
    {
        private static readonly EasLog logger = IEasLog.CreateLogger("RProxy");
        public RProxy()
        {
        }

        public void AddForwarder(ReverseProxyConfig proxy)
        {
            var forwarder = new PortForwarder(proxy);
            ForwarderMap.Add(proxy.Host, forwarder);
            forwarder.StartServer();
        }

        public static Dictionary<IpInfo, PortForwarder> ForwarderMap { get; private set; } = new();

        public IEnumerable<PortForwarder> Forwarders
        {
            get { return ForwarderMap.Values; }
        }
        public static List<RProxy> LoadFromConfig()
        {
            var config = ReverseProxiesConfig.This;
            var proxies = new List<RProxy>();
            foreach (ReverseProxyConfig proxyConfig in config.Proxies)
            {
                var proxy = new RProxy();
                proxy.AddForwarder(proxyConfig);
                proxies.Add(proxy);
            }
            return proxies;
        }

    }
}