using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Navigation
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
                .Add(T["More"], int.MaxValue - 5, nav => nav
                        .IconCss("fal fa-bars")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["More"]}
                        })
                        .Add(T["Users"], int.MaxValue - 1, installed => installed
                            .Action("Index", "Home", "Plato.Users")
                            .IconCss("fal fa-user mr-2")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        ), new List<string>() { "dropdown-toggle-no-caret", "navigation", "text-hidden"}
                );



            //builder
            //    .Add(T["Users"], int.MaxValue - 5, installed => installed
            //            .Action("Index", "Home", "Plato.Users")
            //            .IconCss("fal fa-user")
            //            //.Permission(Permissions.ManageRoles)
            //            .Attributes(new Dictionary<string, object>()
            //            {
            //                {"data-provide", "tooltip"},
            //                {"title", T["Users"]}
            //            })
            //            .LocalNav()
            //        , new List<string>() {"users", "text-hidden"});
        }
    }

}
