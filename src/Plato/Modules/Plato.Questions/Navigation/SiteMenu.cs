using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Navigation
{
    public class SiteMenu : INavigationProvider
    {
        public SiteMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            
            builder
                .Add(T["Questions"], 4, discuss => discuss
                        .IconCss("fal fa-question-circle")
                        .Action("Index", "Home", "Plato.Questions")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"title", T["Questions"]}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                       , new List<string>() { "questions", "text-hidden" }
                );
            
        }

    }

}
