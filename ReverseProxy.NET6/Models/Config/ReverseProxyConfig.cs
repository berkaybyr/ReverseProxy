using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReverseProxy.NET6.Models.Config
{
    [Serializable, XmlRoot("ReverseProxy")]
    public class ReverseProxyConfig
    {
        [XmlElement("Host")]
        public HostInfo Host { get; set; }
        
        [XmlElement("Forward")]
        public ForwardInfo Forward { get; set; }

    }
}