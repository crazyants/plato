using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.Navigation
{

    public class AdminMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }
        
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }
        
        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Users"], int.MaxValue - 4, users => users
                    .IconCss("fal fa-users")
                    .Add(T["Manage"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Users")
                        .Permission(Permissions.ManageUsers)
                        .LocalNav())
                    .Add(T["Add"], 2, create => create
                        .Action("Create", "Admin", "Plato.Users")
                        .Permission(Permissions.AddUsers)
                        .LocalNav())
                );

        }

    }

}
