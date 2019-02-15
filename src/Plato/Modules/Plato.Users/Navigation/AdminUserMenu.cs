using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class AdminUserMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminUserMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin-user", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Users"], int.MaxValue - 4, users => users
                    .IconCss("fal fa-users")
                    .Add(T["Manage"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Add"], 2, create => create
                        .Action("Create", "Admin", "Plato.Users")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));



        }
    }

}
