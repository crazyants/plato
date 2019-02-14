using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Roles.Navigation
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
                .Add(T["Roles"], int.MaxValue - 3, roles => roles
                    .IconCss("fal fa-lock")
                    .Add(T["Manage"], 3, manage => manage
                        .Action("Index", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Add"], 4, create => create
                        .Action("Create", "Admin", "Plato.Roles")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


            //builder
            //    .Add(T["Settings"], 9999, settings => settings
            //        .Add(T["Role Settings"], 4, roles => roles
            //            .Action("Index", "Admin", "Plato.Roles")
            //            //.Permission(Permissions.ManageRoles)
            //            .LocalNav()
            //        ));
        }
    }

}
