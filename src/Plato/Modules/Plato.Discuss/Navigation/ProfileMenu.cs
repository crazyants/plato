using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Navigation
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
                .Add(T["Discuss"], 2, discuss => discuss
                    .Add(T["Topics"], 1, topics => topics
                        .Action("Index", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Favourites"], 999, favourites => favourites
                        .Action("Channels", "Admin", "Plato.Discuss")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
        }
    }

}
