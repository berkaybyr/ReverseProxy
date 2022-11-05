using System.Collections.Generic;
using System.Xml.Serialization;

namespace ReverseProxy.NET6.Models.Config
{
    [Serializable, XmlRoot("ReverseProxiesConfig")]
    public class ReverseProxiesConfig
    {
        private ReverseProxiesConfig() { }

        private static ReverseProxiesConfig instance = null;
        public static ReverseProxiesConfig This
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("Not initalized!");
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        [XmlElement("ReverseProxy")]
        public List<ReverseProxyConfig> Proxies { get; set; }
    }
}