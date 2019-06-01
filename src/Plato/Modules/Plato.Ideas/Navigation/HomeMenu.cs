using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
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
                .Add(T["Ideas"], 5, docs => docs
                    .View("CoreIdeasMenu")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
