using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
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
                .Add(T["Discuss"], configuration => configuration
                    .IconCss("fal fa-comment-alt fa-flip-y")
                    .Add(T["Latest"], int.MinValue, installed => installed
                        .Action("Index", "Home", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    )
                    .Add(T["Popular"], int.MinValue + 1, installed => installed
                        .Action("Popular", "Home", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ), new List<string>() { "discuss" }
                );

        }
    }

}
