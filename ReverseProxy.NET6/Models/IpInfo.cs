using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReverseProxy.NET6.Models
{
    public class IpInfo
    {
        [XmlAttribute("IpAddress")]
        public string IpAddress { get; set; }

        [XmlAttribute("Port")]
        public int Port { get; set; }
    }
}
