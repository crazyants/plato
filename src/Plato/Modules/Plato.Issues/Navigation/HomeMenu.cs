using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Navigation
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
                .Add(T["Issues"], 6, issues => issues
                    .View("CoreIssuesMenu")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );

        }
    }

}
