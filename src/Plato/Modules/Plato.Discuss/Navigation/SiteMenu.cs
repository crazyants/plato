using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
{
    public class SiteMenu : INavigationProvider
    {


        public IStringLocalizer T { get; set; }

        public SiteMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {
            if (!String.Equals(name, "site", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }


            builder
                .Add(T["Discuss"], 1, discuss => discuss
                        .IconCss("fal fa-comment-alt fa-flip-y")
                        .Action("Index", "Home", "Plato.Discuss")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-provide", "tooltip"},
                            {"data-placement", "bottom"},
                            {"title", T["Discuss"]}
                        })
                        .LocalNav(), new List<string>() {"discuss", "text-hidden"}
                );
            
        }

    }

}
