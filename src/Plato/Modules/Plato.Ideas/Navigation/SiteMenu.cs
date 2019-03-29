using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
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
                .Add(T["Ideas"], 4, ideas => ideas
                        .IconCss("fal fa-lightbulb")
                        .Action("Index", "Home", "Plato.Ideas")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Ideas"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                       , new List<string>() { "questions", "text-hidden" }
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
            //                .Action("Index", "Home", "Plato.Ideas")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Ideas")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() {"discuss", "text-hidden"}
            //    );

        }
    }

}
