using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Navigation
{
    public class SearchMenu : INavigationProvider
    {

        private IStringLocalizer T { get; set; }

        private readonly IFeatureFacade _featureFacade;
   
        public SearchMenu(IStringLocalizer<AdminMenu> localizer, IContextFacade contextFacade, IFeatureFacade featureFacade)
        {
            T = localizer;
            _featureFacade = featureFacade;
        }

        public void BuildNavigation(string name, INavigationBuilder builder)
        {

            if (!String.Equals(name, "search", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Get view model from context
            var indexViewModel =
                builder.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;
            if (indexViewModel == null)
            {
                return;
            }

            var feature = _featureFacade.GetFeatureByIdAsync("Plato.Discuss")
                .GetAwaiter()
                .GetResult();

            if (feature == null)
            {
                return;
            }

            builder
                .Add(T["Topics"], 1, topics => topics
                        .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                        {
                            ["opts.featureId"] = feature.Id,
                            ["opts.within"] = "topics",
                            ["opts.search"] = indexViewModel.Options.Search
                        })
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-feature-id", feature.Id}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav(),
                    indexViewModel.Options.FeatureId == feature.Id && indexViewModel.Options.Within == "topics"
                        ? new string[] {"active"}
                        : null
                ).Add(T["Replies"], 3, f => f
                        .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                        {
                            ["opts.featureId"] = feature.Id,
                            ["opts.within"] = "replies",
                            ["opts.search"] = indexViewModel.Options.Search
                        })
                        .Attributes(new Dictionary<string, object>()
                        {
                            {"data-feature-id", feature.Id}
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav(),
                    indexViewModel.Options.FeatureId == feature.Id && indexViewModel.Options.Within == "replies"
                        ? new string[] {"active"}
                        : null
                );
        }

    }

}
