using System;
using System.Collections.Generic;
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
                .Add(T["Everything"], int.MinValue, favourites => favourites
                    .Attributes(new Dictionary<string, object>()
                    {
                        { "data-feature-id", 0 }
                    })
                    .LocalNav()
                );
        }
    }

}
