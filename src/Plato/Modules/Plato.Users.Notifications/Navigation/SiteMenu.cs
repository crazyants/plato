using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;

namespace Plato.Users.Notifications.Navigation
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
                .Add(T["Notifications"], int.MaxValue, configuration => configuration
                        .IconCss("fal fa-bell")
                        .Action("Index", "Home", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Notifications"]}
                        }).LocalNav(), new List<string>() {"notifications", "text-hidden"}
                );

        }
    }

}
