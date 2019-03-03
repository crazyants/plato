using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Navigation
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
                .Add(T["Profile"], 1, profile => profile
                    .Action("EditProfile", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Account"], 2, profile => profile
                    .Action("EditAccount", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                ).Add(T["Settings"], int.MaxValue, profile => profile
                    .Action("EditSettings", "Home", "Plato.Users")
                    //.Permission(Permissions.ManageUsers)
                    .LocalNav()
                );

        }
        
    }

}
