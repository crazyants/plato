using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Facebook.Navigation
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
                    .Add(T["Facebook"], int.MaxValue - 500, webApiSettings => webApiSettings
                        .Action("Index", "Admin", "Plato.Facebook")
                        .Permission(Permissions.EditFacebookSettings)
                        .LocalNav())
                );

        }

    }

}
