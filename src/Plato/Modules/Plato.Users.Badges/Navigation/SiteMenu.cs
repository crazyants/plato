using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Badges.Navigation
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
                        .Add(T["Badges"], int.MaxValue, installed => installed
                            .Action("Index", "Home", "Plato.Users.Badges")
                            .IconCss("fal fa-trophy mr-2")
                            .LocalNav()
                        ), new List<string>() { "dropdown-toggle-no-caret", "navigation", "text-hidden" }
                );

        }

    }

}
