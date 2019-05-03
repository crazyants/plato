using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Tags.Navigation
{

    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer localizer)
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
                    .Add(T["Tags"], 4, manage => manage
                        .Action("Index", "Admin", "Plato.Ideas.Tags")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            
        }

    }

}
