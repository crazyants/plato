using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.Discussions
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
                .Add(T["Discussions"], "1", home => home
                    .Action("Index", "Admin", "Plato.Discussions")
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );


        }
    }

}
