using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Users.Navigation
{
    public class EditProfileMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public EditProfileMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "editprofile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Profile"], 1, profile => profile
                    .Action("EditProfile", "Profile", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Account"], 2, profile => profile
                    .Action("EditAccount", "Profile", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Settings"], int.MaxValue, profile => profile
                    .Action("EditSettings", "Profile", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
        
    }

}
