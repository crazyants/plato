using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Media
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

            //builder
            //    .Add(T["Media"], "1", 1, home => home
            //        .Action("Index", "Admin", "Plato.Media")
            //        //.Permission(Permissions.ManageRoles)
            //        .LocalNav()
            //    );


        }
    }

}
