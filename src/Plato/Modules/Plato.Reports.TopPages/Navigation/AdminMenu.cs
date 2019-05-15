using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Reports.TopPages.Navigation
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
                .Add(T["Reports"], int.MaxValue - 4, questions => questions
                    .IconCss("fal fa-chart-bar")
                    .Add(T["Top Pages"], 3, home => home
                        .Action("Index", "Admin", "Plato.Reports.TopPages")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav())
                );

        }

    }

}
