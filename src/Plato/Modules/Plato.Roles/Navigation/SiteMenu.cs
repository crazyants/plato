using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Roles.Navigation
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer<SiteMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            //builder
            //    .Add(T["Articles"], configuration => configuration
            //        .Add(T["Roles 2"], "5", security => security
            //            .Add(T["Roles 3"], "10", installed => installed
            //                .Action("Index", "Admin", "Plato.Roles")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )), new List<string>() { "roles" });

        }
    }

}
