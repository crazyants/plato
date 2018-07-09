using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Email
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
                .Add(T["Settings"], 9999, configuration => configuration
                    .Add(T["Email Settings"], 2, installed => installed
                        .Action("Index", "Admin", "Plato.Email")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));

        }
    }

}
