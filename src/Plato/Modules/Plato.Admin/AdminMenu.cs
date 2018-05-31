using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Navigation;

namespace Plato.Admin
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Home"], "1", home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );


        }
    }

}
