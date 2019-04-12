using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Navigation
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
                .Add(T["Docs"], 2, discuss => discuss
                        .IconCss("fal fa-book-open")
                        .Action("Index", "Home", "Plato.Docs")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Docs"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav(), new List<string>() {"docs", "text-hidden"}
                );
            
            //builder
            //    .Add(T["Discuss"], 1, discuss => discuss
            //            .IconCss("fal fa-comment-alt fa-flip-y")
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Discuss"]}
            //            })
            //            .Add(T["Latest"], int.MinValue, installed => installed
            //                .Action("Index", "Home", "Plato.Docs")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            )
            //            .Add(T["Popular"], int.MinValue + 1, installed => installed
            //                .Action("Popular", "Home", "Plato.Docs")
            //                //.Permission(Permissions.ManageRoles)
            //                .LocalNav()
            //            ), new List<string>() {"discuss", "text-hidden"}
            //    );

        }
    }

}
