using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Users.reCAPTCHA2.Navigation
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
                    .Add(T["reCAPTCHA2"], int.MaxValue - 98, installed => installed
                        .Action("Index", "Admin", "Plato.Users.reCAPTCHA2")
                        //.Permission(Permissions.ManageUsers)
                        .LocalNav()
                    ));

        }

    }

}
