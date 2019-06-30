using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Categories.Permissions.Navigation
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Discuss"], 1, users => users
                    .Add(T["Moderators"], int.MaxValue, manage => manage
                        .Action("Index", "Admin", "Plato.Discuss.Categories.Permissions")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));


        }
    }

}
