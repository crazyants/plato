using Microsoft.Extensions.Localization;
using System;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Navigation
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
                .Add(T["Questions"], 3, users => users
                    .IconCss("fal fa-question-circle")
                    .Add(T["Overview"], int.MinValue, home => home
                        .Action("Index", "Admin", "Plato.Questions")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));
            

        }
    }

}
