using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

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
                .Add(T["Users"], 9998, roles => roles
                    .Add(T["Manage Roles"], 3, manage => manage
                        .Action("Index", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Create Role"], 4, create => create
                        .Action("CreateRole", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


            builder
                .Add(T["Settings"], 9999, settings => settings
                    .Add(T["Roles"], 2, roles => roles
                        .Action("Index", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
        }
    }

}
