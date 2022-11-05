using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ReverseProxy.NET6.Lib
{
    public static class XmlHandler
    {
        private static readonly string _curPath = Directory.GetCurrentDirectory();

        private static string _xmlSettingsPath
        {
            get
            {
                return _curPath + "\\ReverseProxy.xml";
            }
        }
        public static void Read()
        {
            var xDoc = XDocument.Load(_xmlSettingsPath);
            if (xDoc == null) throw new Exception("XDoc is NULL");
            var xRoot = xDoc.Root;
            if (xRoot == null) throw new Exception("XRoot is NULL");
            var xProxy = xRoot.Element("ReverseProxiesConfig");
            ReverseProxiesConfig.This = XmlDeserialize<ReverseProxiesConfig>(xProxy);
        }
        private static T? XmlDeserialize<T>(this XElement xElement)
        {
            StringReader reader = new(xElement.ToString().Replace("True", "true").Replace("False", "false"));
            XmlSerializer xmlSerializer = new(typeof(T));
            var item = (T?)xmlSerializer.Deserialize(reader);
            return item;
        }
    }
}
