using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
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
            var searchIndexViewModel = builder.ActionContext.HttpContext.Items[typeof(SearchIndexViewModel)] as SearchIndexViewModel;
            if (searchIndexViewModel == null)
            {
                return;
            }

            builder
                .Add(T["All"], int.MinValue, f => f
                        .Action("Index", "Home", "Plato.Search")
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-feature-id", 0}
                        })
                        .LocalNav(), searchIndexViewModel.Options.FeatureId == 0
                        ? new string[] {"active"}
                        : null);
        }
    }

}
