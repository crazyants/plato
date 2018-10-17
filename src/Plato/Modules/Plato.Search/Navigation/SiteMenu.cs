using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

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
            
            builder
                .Add(T["Search"], int.MaxValue, installed => installed
                    .Action("Index", "Home", "Plato.Search")
                    //.Permission(Permissions.ManageRoles)
                        .IconCss("fal fa-search")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Search"]}
                        })
                    .LocalNav()
                , new List<string>() { "search", "text-hidden"});

        }
    }

}
