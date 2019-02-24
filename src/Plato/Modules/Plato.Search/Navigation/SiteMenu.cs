using System;
using System.Collections.Generic;
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
            
            //builder
            //    .Add(T["Search"], 3, search => search
            //        .Action("Index", "Home", "Plato.Search")
            //        //.Permission(Permissions.ManageRoles)
            //            .IconCss("fal fa-search")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Search"]}
            //            })
            //        .LocalNav()
            //    , new List<string>() { "search", "text-hidden"});


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
