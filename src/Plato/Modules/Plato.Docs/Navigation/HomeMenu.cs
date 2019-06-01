using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Navigation
{
    public class HomeMenu : INavigationProvider
    {
        
        public IStringLocalizer T { get; set; }

        public HomeMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "home", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Docs"], 2, docs => docs
                    .View("CoreDocsMenu")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
