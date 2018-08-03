using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class ProfileMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public ProfileMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "userprofile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Profile"], 1, profile => profile
                    .Action("Edit", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Account"], 2, profile => profile
                    .Action("Account", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Settings"], int.MaxValue, profile => profile
                    .Action("Settings", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
    }

}
