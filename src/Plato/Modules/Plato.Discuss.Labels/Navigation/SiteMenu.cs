using Microsoft.Extensions.Localization;
using System;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Labels.Navigation
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

            //builder
            //    .Add(T["Discuss"], configuration => configuration
            //        .Add(T["Labels"], 2, installed => installed
            //            .Action("Index", "Home", "Plato.Discuss.Labels", new RouteValueDictionary()
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
