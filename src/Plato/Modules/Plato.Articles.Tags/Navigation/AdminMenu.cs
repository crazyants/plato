using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Tags.Navigation
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
                .Add(T["Articles"], 3, users => users
                    .IconCss("fal fa-copy")
                    .Add(T["Tags"], 4, manage => manage
                        .Action("Index", "Admin", "Plato.Articles.Tags")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            
        }

    }

}
