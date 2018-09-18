using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUserAsync();

        Task<ISiteSettings> GetSiteSettingsAsync();

        Task<string> GetBaseUrlAsync();

        string GetRouteUrl(RouteValueDictionary routeValues);

        Task<string> GetCurrentCultureAsync();

        string GetCurrentCulture();

    }

}
