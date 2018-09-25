using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.WebApi.Navigation
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Settings"], 9999, configuration => configuration
                    .IconCss("fal fa-cog")
                    .Add(T["Web Api Settings"], 6, webApiSettings => webApiSettings
                        .Action("Index", "Admin", "Plato.WebApi")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));

        }
    }

}
