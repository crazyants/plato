using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Shell.Abstractions
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUserAsync();

        Task<ShellModule> GetFeatureByAreaAsync();

        Task<ShellModule> GetFeatureByModuleIdAsync(string areaName);

        Task<ISiteSettings> GetSiteSettingsAsync();

        Task<string> GetBaseUrlAsync();

        string GetRouteUrl(RouteValueDictionary routeValues);

        Task<CultureInfo> GetCurrentCulture();

    }

}
