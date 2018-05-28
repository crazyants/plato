﻿using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Navigation;

namespace Plato.Roles
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer<AdminMenu> localizer)
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
                .Add(T["Roles 1"], configuration => configuration
                    .Add(T["Roles 2"], "5", security => security
                        .Add(T["Roles 3"], "10", installed => installed
                            .Action("Index", "Admin", "OrchardCore.Roles")
                            //.Permission(Permissions.ManageRoles)
                            .LocalNav()
                        )));
        }
    }

}