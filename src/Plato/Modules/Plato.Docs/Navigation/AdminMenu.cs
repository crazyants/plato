using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Navigation
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
                .Add(T["Docs"], 2, docs => docs
                    .IconCss("fal fa-book-open")
                    .Add(T["Overview"], int.MinValue, home => home
                        .Action("Index", "Admin", "Plato.Docs")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            

        }
    }

}
