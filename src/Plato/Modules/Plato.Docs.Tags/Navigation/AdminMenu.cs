using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Tags.Navigation
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
                .Add(T["Docs"], 2, docs => docs
                    .IconCss("fal fa-book-open")
                    .Add(T["Tags"], 4, tags => tags
                        .Action("Index", "Admin", "Plato.Docs.Tags")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            
        }

    }

}
