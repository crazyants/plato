using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
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

        public void BuildNavigation(string name, INavigationBuilder builder)
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
                        options = new EntityIndexOptions()
                    })
                    //.Permission(Permissions.ReactToTopics)
                );



        }
    }

}
