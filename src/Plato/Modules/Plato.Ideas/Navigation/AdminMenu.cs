using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
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
                .Add(T["Ideas"], 5, ideas => ideas
                    .IconCss("fal fa-lightbulb")
                    .Add(T["Overview"], int.MinValue, home => home
                        .Action("Index", "Admin", "Plato.Ideas")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));

        }

    }

}
