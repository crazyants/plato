using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface ICapturedRouterUrlHelper
    {

        Task<string> GetBaseUrlAsync();

        string GetRouteUrl(string baseUri, RouteValueDictionary routeValues);

        string GetRouteUrl(Uri baseUri, RouteValueDictionary routeValues);
        
    }

}
