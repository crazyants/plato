using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Plato.Internal.Navigation;

namespace Plato.Users.Badges.Navigation
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
                .Add(T["Badges"], 3, badges => badges
                        .Action("Index", "Home", "Plato.Users.Badges")
                        .IconCss("fal fa-trophy")
                        //.Permission(Permissions.ManageRoles)
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Badges"]}
                        })
                        .LocalNav()
                    , new List<string>() { "badges", "text-hidden" });
        }
    }

}
