using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.StopForumSpam.Navigation
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
                .Add(T["SPAM"], int.MaxValue - 1, configuration => configuration
                    .IconCss("fal fa-exclamation-triangle")
                    .Add(T["StopForumSpam"], 2, installed => installed
                        .Action("Index", "Admin", "Plato.StopForumSpam")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));


        }
    }

}
