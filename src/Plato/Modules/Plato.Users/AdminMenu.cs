using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users
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
                .Add(T["Users"], "5", users => users
                    .Add(T["Manage Users"], "1", manage => manage
                        .Action("Index", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Create User"], "2", create => create
                        .Action("Create", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


            builder
                .Add(T["Settings"], "9999", configuration => configuration
                    .Add(T["Users"], "1", installed => installed
                        .Action("Index", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));

        }
    }

}
