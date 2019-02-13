using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Net.Abstractions;

namespace Plato.Internal.Net
{

    public class ClientIpAddress : IClientIpAddress
    {
        public const string ForwardForHeader = "X-Forwarded-For";
        public const string RemoteAddressHeader = "REMOTE_ADDR";


        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientIpAddress(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIpV4Address(bool tryUseXForwardHeader = true)
        {

            var value = string.Empty;

            // Check X-Forwarded-For
            // Is the request forwarded via a proxy?
            if (tryUseXForwardHeader)
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    value = _httpContextAccessor.GetRequestHeaderValueAs<string>(ForwardForHeader)?.Split(',').FirstOrDefault();
                }
            }

            // If no "X-Forwarded-For" header, get "REMOTE_ADDR" header instead
            if (String.IsNullOrEmpty(value))
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    value = _httpContextAccessor.GetRequestHeaderValueAs<string>(RemoteAddressHeader);
                }
            }

            // Nothing yet, check .NET core implementation (HttpContext.Connection)
            if (String.IsNullOrEmpty(value))
            {
                // This can sometimes be null in earlier versions of .NET core
                if (_httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress != null)
                {
                    value = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
            }

            // Translate to valid IP 
            IPAddress ip = null;
            if (!String.IsNullOrEmpty(value))
            {
                // Attempt to parse IP address
                IPAddress.TryParse(value, out ip);
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ip = Dns.GetHostEntry(ip).AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
            }

            return ip == null ? string.Empty : ip.ToString();

        }

        public string GetIpV6Address()
        {
            var ip = GetIpV4Address();
            if (!String.IsNullOrEmpty(ip))
            {
                var addressList = Dns.GetHostEntry(ip).AddressList;
                if (addressList != null)
                {
                    ip = addressList.FirstOrDefault()?.ToString();
                }
            }
            return ip;
        }

    }
  
}
