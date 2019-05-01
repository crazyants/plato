using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Navigation
{

    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Issues"], 7, users => users
                    .IconCss("fal fa-bug")
                    .Add(T["Overview"], int.MinValue, home => home
                        .Action("Index", "Admin", "Plato.Issues")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            

        }
    }

}
