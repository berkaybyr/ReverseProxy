using EasMe;
using EasMe.Extensions;
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
        private static readonly EasLog logger = IEasLog.CreateLogger("XmlHandler");
#if DEBUG || WINDOWS
        private static string _xmlSettingsPath
        {
            get
            {
                return _curPath + "\\ReverseProxy.xml";
            }
        }
#elif RELEASE || LINUX
        private static string _xmlSettingsPath
        {
            get
            {
                return _curPath + "/ReverseProxy.xml";
            }
        }
#endif
        public static void ReadXml()
        {
            try
            {
                logger.Info("Reading XML file: " + _xmlSettingsPath);
                var xDoc = XDocument.Load(_xmlSettingsPath);
                if (xDoc == null) throw new Exception("XDoc is NULL");
                var xRoot = xDoc.Root;
                if (xRoot == null) throw new Exception("XRoot is NULL");
                var xProxy = xRoot.Element("ReverseProxiesConfig");
                ReverseProxiesConfig.This = xProxy.XmlDeserialize<ReverseProxiesConfig>();
                ValidateXmlData();
            }
            catch(Exception ex)
            {
                logger.Exception(ex, "ReadXml");
                Environment.Exit(-1);
            }
        }
        
        private static void ValidateXmlData()
        {
            var config = ReverseProxiesConfig.This;
            if (config == null) throw new NullReferenceException("ReverseProxiesConfig is NULL");
            if (config.Proxies.Count == 0) throw new InvalidDataException("No reverse proxy configuration found inside ReverseProxy.xml");
            config.Proxies.ForEach(x =>
            {
                x.Host.IpAddress = x.Host.IpAddress.Trim();
                x.Forward.IpAddress = x.Forward.IpAddress.Trim();
                if (!x.Host.IpAddress.IsValidIPAddress()) throw new InvalidDataException("Host Ip Address " + x.Host.IpAddress + " is not valid");
                if (!x.Forward.IpAddress.IsValidIPAddress()) throw new InvalidDataException("Forward Ip Address " + x.Forward.IpAddress + " is not valid");
                if (!x.Host.Port.IsValidPort()) throw new InvalidDataException("Host Port " + x.Host.Port + " is not valid");
                if (!x.Forward.Port.IsValidPort()) throw new InvalidDataException("Forward Port " + x.Forward.Port + " is not valid");
                x.FilterConnection?.IpAddresses?.ForEach(x =>
                {
                    if (!x.IsValidIPAddress()) throw new InvalidDataException("FilterConnection Ip Address " + x + " is not valid");
                });
                if (x.ConnectionLimitPerIp < 1) throw new InvalidDataException("ConnectionLimitPerIp cannot be less than 1");
                if(x.ReceiveTimeout < 1) throw new InvalidDataException("ReceiveTimeout cannot be less than 1");
                if(x.SendTimeout < 1) throw new InvalidDataException("SendTimeout cannot be less than 1");
            });

        }
    }
}
