using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.ViewModels;

namespace Plato.Search.Navigation
{
    public class SearchMenu : INavigationProvider
    {

        public IStringLocalizer T { get; set; }

        public SearchMenu(IStringLocalizer localizer)
        {
            T = localizer;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "search", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get view model from context
            var indexViewModel = builder.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel)] as EntityIndexViewModel;
            if (indexViewModel == null)
            {
                return;
            }

            builder
                .Add(T["All"], 0, f => f
                        .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                        {
                            ["opts.FeatureId"] = null,
                            ["opts.Within"] =string.Empty,
                            ["opts.Search"] = indexViewModel.Options.Search
                        })
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-feature-id", 0}
                        })
                        .LocalNav(), indexViewModel.Options.FeatureId == 0
                        ? new string[] {"active"}
                        : null);
        }
    }

}
