using System.Collections.Generic;

namespace ReverseProxy.Models.Config
{
    public class ReverseProxiesConfig
    {
        public List<ReverseProxyConfig> Proxies { get; set; }
    }
}