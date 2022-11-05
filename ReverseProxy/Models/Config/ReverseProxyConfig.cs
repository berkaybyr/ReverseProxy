using System.Collections.Generic;

namespace ReverseProxy.Models.Config
{
    public class ReverseProxyConfig
    {
        public IpInfo Host { get; set; }
        public IpInfo Forward { get; set; }
    }
}