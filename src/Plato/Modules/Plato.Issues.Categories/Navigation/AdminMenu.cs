using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Categories.Navigation
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
                .Add(T["Issues"], 7, issues => issues
                    .IconCss("fal fa-bug")
                    .Add(T["Categories"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Issues.Categories")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            
        }

    }

}
