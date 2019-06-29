using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Email.Navigation
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
                .Add(T["Settings"], int.MaxValue, configuration => configuration
                    .IconCss("fal fa-cog")
                    .Add(T["Email"], int.MinValue + 25, installed => installed
                        .Action("Index", "Admin", "Plato.Email")
                        .Permission(Permissions.ManageEmailSettings)
                        .LocalNav()
                    ));

        }

    }

}
