using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Navigation.Abstractions;

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
            var indexViewModel = builder.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;
            if (indexViewModel == null)
            {
                return;
            }

            // Get metrics from context
            var model =
                builder.ActionContext.HttpContext.Items[typeof(FeatureEntityCounts)] as
                    FeatureEntityCounts;
            
            // Get feature metrics
            var total = model?.Total() ?? 0;

            builder
                .Add(T["All"], 0, f => f
                        .Badge(total > 0 ? total.ToPrettyInt() : string.Empty, "badge badge-primary float-right")
                        .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                        {
                            ["opts.featureId"] = null,
                            ["opts.search"] = indexViewModel.Options.Search
                        })
                        .LocalNav(), indexViewModel.Options.FeatureId == null
                        ? new string[] {"active"}
                        : null);

        }

    }

}
