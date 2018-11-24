using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Hosting.Abstractions
{
    
    public interface ICapturedRouter
    {

        ICapturedRouter Configure(Action<CapturedRouterOptions> configure);

        Task<string> GetBaseUrlAsync();

        string GetRouteUrl(string baseUri, RouteValueDictionary routeValues);

        string GetRouteUrl(Uri baseUri, RouteValueDictionary routeValues);

    }
    
    public class CapturedRouterOptions
    {

        public IRouter Router { get; set; }

        public string BaseUrl { get; set; }

    }
    
}
