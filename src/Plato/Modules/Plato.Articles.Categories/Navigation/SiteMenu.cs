using Microsoft.Extensions.Localization;
using System;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.Navigation
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

            builder
                .Add(T["Articles"], configuration => configuration
                    .Add(T["Categories"], 1, categories => categories
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    )
                );

        }
    }

}
