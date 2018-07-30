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
                .Add(T["Users"], configuration => configuration

                    .Add(T["Member List"], "1", installed => installed
                        .Action("Index", "Home", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
        }
    }

}
