using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
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
                .Add(T["Ideas"], 5, ideas => ideas
                        .IconCss("fal fa-lightbulb")
                        .Action("Index", "Home", "Plato.Ideas")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"data-placement", "bottom"},
                            {"title", T["Ideas"]}
                        })
                        .LocalNav()
                    , new List<string>() {"ideas", "text-hidden"}
                );

        }

    }

}
