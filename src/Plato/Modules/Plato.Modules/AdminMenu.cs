using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Modules
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
                .Add(T["Modules"], users => users
                    .Add(T["Manage Modules"], "15", manageusers => manageusers
                        .Action("Index", "Admin", "Plato.Modules")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Module Gallary"], "16", roles => roles
                        .Action("Create", "Admin", "Plato.Modules")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


        }
    }

}
