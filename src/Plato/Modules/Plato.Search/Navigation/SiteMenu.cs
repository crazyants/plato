using System;
using Microsoft.Extensions.Localization;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.ViewModels;
using System.Collections.Generic;

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
            
            builder
                .Add(T["More"], int.MaxValue - 5, nav => nav
                        .IconCss("fal fa-bars")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"data-placement", "bottom"},
                            {"title", T["More"]}
                        })
                        .Add(T["Search"], 0, search => search
                            .Action("Index", "Home", "Plato.Search")
                            .IconCss("fal fa-search mr-2")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ).Add(T["Divider"], 1, divider => divider
                            //.Permission(deletePermission)
                            .DividerCss("dropdown-divider").LocalNav()
                        )

                    , new List<string>() { "dropdown-toggle-no-caret", "navigation", "text-hidden" }
                );





        }
    }

}
