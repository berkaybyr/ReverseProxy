using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReverseProxy.NET6.Models.Config
{
    [Serializable, XmlRoot("FilterConnection")]
    public class FilterConnection
    {
        [XmlElement("IpAddress")]
        public List<string>? IpAddresses { get; set; }
    }
}
