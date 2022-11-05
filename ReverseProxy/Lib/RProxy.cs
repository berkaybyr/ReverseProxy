using ReverseProxy.Models;
using ReverseProxy.Models.Config;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace ReverseProxy.Lib
{
    public class RProxy
    {
        public RProxy()
        {
            ForwarderMap = new Dictionary<IpInfo, PortForwarder>();
        }

        public void AddForwarder(ReverseProxyConfig proxy)
        {
            var forwarder = new PortForwarder(proxy);
            ForwarderMap.Add(proxy.Host, forwarder);
            forwarder.StartServer();
        }

        public Dictionary<IpInfo, PortForwarder> ForwarderMap { get; private set; }

        public IEnumerable<PortForwarder> Forwarders
        {
            get { return ForwarderMap.Values; }
        }

        public static List<RProxy> LoadFromConfig()
        {
            var config = (ReverseProxiesConfig)ConfigurationManager.GetSection("ReverseProxiesConfig");

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