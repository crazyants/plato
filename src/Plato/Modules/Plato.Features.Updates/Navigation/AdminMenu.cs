using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Features.Updates.Navigation
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
                .Add(T["Features"], int.MaxValue - 5, features => features
                    .IconCss("fal fa-cube")
                    .Add(T["Update Features"], 2, manage => manage
                        .Action("Index", "Admin", "Plato.Features.Updates")
                        .Permission(Permissions.ManageFeatureUpdates)
                        .LocalNav())
                );

        }

    }

}
