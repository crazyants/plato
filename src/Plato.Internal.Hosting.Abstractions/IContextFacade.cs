using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUserAsync();

        Task<ISiteSettings> GetSiteSettingsAsync();

        Task<string> GetBaseUrlAsync();
        
        Task<string> GetBaseUrlAsync(HttpRequest request);

        string GetRouteUrl(RouteValueDictionary routeValues);

        Task<string> GetCurrentCultureAsync();
        
    }

}
