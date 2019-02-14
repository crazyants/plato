using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Theming.Navigation
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
                .Add(T["Theming"], int.MaxValue - 2, users => users
                    .IconCss("fal fa-palette fa-flip-y")
                    .Add(T["Manage Themes"], 1, create => create
                        .Action("Index", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    )
                    .Add(T["Theme Gallery"], 1, create => create
                        .Action("Index", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));



        }
    }

}
