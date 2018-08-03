using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
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
                .Add(T["Discuss"], 1, users => users
                    .Add(T["Latest"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["settings"], 999, create => create
                        .Action("Channels", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            

        }
    }

}
