using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Navigation;

namespace Plato.Users
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

            //builder
            //    .Add(T["Configuration"], configuration => configuration
            //        .Add(T["Security"], "5", security => security
            //            .Add(T["Users"], "5", installed => installed
            //                .Action("Index", "Admin", "Plato.Users")
            //                //.Permission(Permissions.ManageUsers)
            //                .LocalNav()
            //            )));
            //.Add(T["Settings"], settings => settings
            //    .Add(T["Users"], users => users
            //        //.Permission(Permissions.ManageUsers)
            //        .Action("Index", "Admin", new { area = "Plato.Settings"})
            //        .LocalNav()
            //    )));
        }
    }

}
