using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Navigation
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
            
            builder
                .Add(T["Articles"], 3, discuss => discuss
                        .IconCss("fal fa-copy")
                        .Action("Index", "Home", "Plato.Articles")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Articles"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                       , new List<string>() { "articles",  "text-hidden" }
                );

            //builder
            //    .Add(T["Articles"], 2, discuss => discuss
            //            .IconCss("fal fa-book-open")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Articles"]}
            //            })
            //            .Add(T["Latest"], int.MinValue, installed => installed
            //                .Action("Index", "Home", "Plato.Articles")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Articles")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() {"discuss", "text-hidden"}
            //    );

        }
    }

}
