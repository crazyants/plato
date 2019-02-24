using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;
using Plato.Search.ViewModels;

namespace Plato.Search.Navigation
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Add reaction menu view to navigation
            builder
                .Add(T["SearchMenu"], 3, react => react
                    .View("SearchMenu", new
                    {
                        options = new SearchIndexOptions()
                    })
                    //.Permission(Permissions.ReactToTopics)
                );



        }
    }

}
