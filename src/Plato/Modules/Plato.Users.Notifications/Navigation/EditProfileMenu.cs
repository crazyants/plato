using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Notifications.Navigation
{
    public class EditProfileMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public EditProfileMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "edit-profile", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Notifications"], 4, profile => profile
                    .Action("EditProfile", "Home", "Plato.Users.Notifications")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
        
    }

}
