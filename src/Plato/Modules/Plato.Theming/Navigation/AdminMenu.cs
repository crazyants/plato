using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Theming.Navigation
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
                .Add(T["Theming"], int.MaxValue - 2, theming => theming
                    .IconCss("fal fa-palette fa-flip-y")
                    .Add(T["Manage"], 1, themes => themes
                        .Action("Index", "Admin", "Plato.Theming")
                        .Permission(Permissions.ManageThemes)
                        .LocalNav())
                    .Add(T["Add"], 1, themes => themes
                        .Action("Create", "Admin", "Plato.Theming")
                        .Permission(Permissions.CreateThemes)
                        .LocalNav())
                );
            
        }

    }

}
