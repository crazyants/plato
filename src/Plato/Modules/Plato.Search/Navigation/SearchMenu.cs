using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Search.Navigation
{
    public class SearchMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public SearchMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "search", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["All"], int.MinValue, favourites => favourites
                    .Action("Index", "Home", "Plato.Search")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );
        }
    }

}
