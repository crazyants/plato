using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Features.Navigation
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
                .Add(T["Features"], int.MaxValue - 5, features => features
                    .IconCss("fal fa-cube")
                    .Add(T["Manage Features"], "15", manage => manage
                        .Action("Index", "Admin", "Plato.Features")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Feature Gallery"], "16", gallary => gallary
                        .Action("Create", "Admin", "Plato.Features")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


        }
    }

}
