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

        [XmlElement("FilterConnection")]
        public FilterConnection? FilterConnection { get; set; }

        [XmlAttribute("ReceiveTimeout")]
        public int ReceiveTimeout { get; set; } = 10;

        [XmlAttribute("SendTimeout")]
        public int SendTimeout { get; set; } = 10;

        [XmlAttribute("ConnectionLimitPerIp")]
        public long ConnectionLimitPerIp { get; set; } = 3;

        [XmlElement("RequireConnectionToPort")]
        public RequireConnectionToPort? RequireConnectionToPort { get; set; } 
    }
}