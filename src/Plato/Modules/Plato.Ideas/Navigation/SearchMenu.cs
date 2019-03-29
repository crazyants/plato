using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Navigation
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
           var indexViewModel = builder.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Entity>;
           if (indexViewModel == null)
           {
               return;
           }

           var feature = _featureFacade.GetFeatureByIdAsync("Plato.Ideas")
                .GetAwaiter()
                .GetResult();

            if (feature == null)
            {
                return;
            }

            builder
                .Add(T["Questions"], 2, topics => topics
                    .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                    {
                        ["opts.featureId"] = feature.Id,
                        ["opts.within"] = "questions",
                        ["opts.search"] = indexViewModel.Options.Search
                    })
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-feature-id", feature.Id}
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav(),
                    indexViewModel.Options.FeatureId == feature.Id && indexViewModel.Options.Within == "articles"
                        ? new string[] { "active" }
                        : null
                ).Add(T["Answers"], 4, f => f
                    .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                    {
                        ["opts.featureId"] = feature.Id,
                        ["opts.within"] = "answers",
                        ["opts.search"] = indexViewModel.Options.Search
                    })
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-feature-id", feature.Id}
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav(),
                    indexViewModel.Options.FeatureId == feature.Id && indexViewModel.Options.Within == "comments"
                        ? new string[] { "active" }
                        : null
                );
        }
    }

}
