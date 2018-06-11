using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Users 1"], configuration => configuration
                    .Add(T["Users 2"], "5", security => security
                        .Add(T["Users 3"], "10", installed => installed
                            .Action("Index", "Admin", "Plato.Users")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )));
        }
    }

}
