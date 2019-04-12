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
                .Add(T["Navigation"], int.MaxValue - 5, nav => nav
                        .IconCss("fal fa-bars")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["More"]}
                        })
                        .Add(T["Badges"], int.MaxValue, installed => installed
                            .Action("Index", "Home", "Plato.Users.Badges")
                            .IconCss("fal fa-trophy mr-2")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ), new List<string>() { "dropdown-toggle-no-caret", "navigation", "text-hidden" }
                );


            //builder
            //    .Add(T["Badges"], int.MaxValue - 10, badges => badges
            //            .Action("Index", "Home", "Plato.Users.Badges")
            //            .IconCss("fal fa-trophy")
            //            //.Permission(Permissions.ManageRoles)
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Badges"]}
            //            })
            //            .LocalNav()
            //        , new List<string>() { "badges", "text-hidden" });
        }
    }

}
