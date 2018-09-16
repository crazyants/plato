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
            if (!String.Equals(name, "profile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Profile"], 1, profile => profile
                    .Action("Display", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
    }

}
