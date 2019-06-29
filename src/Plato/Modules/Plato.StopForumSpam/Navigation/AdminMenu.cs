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
                .Add(T["Settings"], int.MaxValue, settings => settings
                    .IconCss("fal fa-cog")
                    .Add(T["StopForumSpam"], int.MaxValue - 99, installed => installed
                        .Action("Index", "Admin", "Plato.StopForumSpam")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));

        }
    }

}
