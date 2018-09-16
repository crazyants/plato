using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class AdminMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Users"], 9996, users => users
                    .Add(T["Manage"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Add"], 2, create => create
                        .Action("Create", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


            //builder
            //    .Add(T["Settings"], 9999, configuration => configuration
            //        .Add(T["User Settings"], 3, installed => installed
            //            .Action("Index", "Admin", "Plato.Users")
            //            //.Permission(Permissions.ManageUsers)
            //            .LocalNav()
            //        ));

        }
    }

}
