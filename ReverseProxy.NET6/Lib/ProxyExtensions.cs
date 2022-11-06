using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReverseProxy.NET6.Lib
{
    public static class ProxyExtensions
    {
        public static string? ToIpAddressString(this string? endPoint)
        {
            var split = endPoint?.Split(':');
            if (split == null) return null;
            if (split.Length == 0) return null;
            var ip = split[0];
            if(IPAddress.TryParse(ip,out var ipModel))
            {
                return ipModel.ToString();
            }
            return null;
        }
    }
}
