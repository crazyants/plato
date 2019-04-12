using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Search.Navigation
{
    public class SiteSearchMenu : INavigationProvider
    {
        public SiteSearchMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "site-search", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            // Add reaction menu view to navigation
            builder
                .Add(T["SearchMenu"], int.MaxValue - 20, react => react
                    .View("SearchMenu", new
                    {
                        options = new EntityIndexOptions()
                    })
                    //.Permission(Permissions.ReactToTopics)
                );
            
        }

    }

}
