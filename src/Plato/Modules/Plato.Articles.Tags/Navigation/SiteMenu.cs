using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Tags.Navigation
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
            //        .Add(T["Tags"], 4, installed => installed
            //            .Action("Index", "Home", "Plato.Articles.Tags", new RouteValueDictionary()
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
