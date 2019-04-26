using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Reports.Navigation
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
                    .Add(T["Overview"], int.MinValue, home => home
                        .Action("Index", "Admin", "Plato.Reports")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav())
                    .Add(T["Page Views"], int.MinValue, home => home
                        .Action("PageViews", "Admin", "Plato.Reports")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav())
                );

        }

    }

}
