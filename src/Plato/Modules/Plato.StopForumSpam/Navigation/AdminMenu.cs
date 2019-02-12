using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;

namespace Plato.StopForumSpam.Navigation
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
                .Add(T["Settings"], int.MaxValue, configuration => configuration
                    .IconCss("fal fa-cog")
                    .Add(T["StopForumSpam"], 2, installed => installed
                        .Action("Index", "Admin", "Plato.StopForumSpam")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));


        }
    }

}
