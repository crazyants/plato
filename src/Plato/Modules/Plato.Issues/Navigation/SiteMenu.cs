using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Navigation
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
                .Add(T["Issues"], 7, issues => issues
                        .IconCss("fal fa-bug")
                        .Action("Index", "Home", "Plato.Issues")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Issues"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                       , new List<string>() { "articles",  "text-hidden" }
                );

        }
    }

}
