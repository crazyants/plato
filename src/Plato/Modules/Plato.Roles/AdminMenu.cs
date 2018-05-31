using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Navigation;

namespace Plato.Roles
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
                .Add(T["Users"], configuration => configuration
                    .Add(T["Manage Roles"], "10", roles => roles
                        .Action("Index", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Add Role"], "10", roles => roles
                        .Action("CreateRole", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));

            builder
                .Add(T["Configuration"], configuration => configuration
                    .Add(T["Security"], "5", security => security
                        .Add(T["Roles"], "10", roles => roles
                            .Action("Index", "Admin", "Plato.Roles")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )
                        .Add(T["Users"], "11", users => users
                            .Action("Index", "Admin", "Plato.Users")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ))
                
                );
        }
    }

}
