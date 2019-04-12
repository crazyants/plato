using System;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Labels.Navigation
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer localizer)
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
                .Add(T["Questions"], 4, questions => questions
                    .IconCss("fal fa-question-circle")
                    .Add(T["Labels"], 3, manage => manage
                        .Action("Index", "Admin", "Plato.Questions.Labels")
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ));

        }

    }

}
