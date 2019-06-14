using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Categories.Navigation
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
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
                    .Add(T["Categories"], 1, manage => manage
                        .Action("Index", "Admin", "Plato.Discuss.Categories")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            

        }
    }

}
