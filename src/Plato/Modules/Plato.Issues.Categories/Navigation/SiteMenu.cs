using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Issues.Categories.Navigation
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer<AdminMenu> localizer)
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

            //builder
            //    .Add(T["Discuss"], configuration => configuration
            //        .Add(T["Channels"], 1, installed => installed
            //            .Action("Index", "Home", "Plato.Issues.Categories", new RouteValueDictionary()
            //            {
            //                ["opts.id"] = "",
            //                ["opts.alias"] = ""
            //            })
            //            //.Permission(Permissions.ManageRoles)
            //            .LocalNav()
            //        )
            //    );

        }
    }

}
