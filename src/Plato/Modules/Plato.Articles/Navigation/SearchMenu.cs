using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Navigation
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
            //var searchIndexViewModel = builder.ActionContext.HttpContext.Items[typeof(SearchIndexViewModel)] as SearchIndexViewModel;
            //if (searchIndexViewModel == null)
            //{
            //    return;
            //}
            
            var feature = _featureFacade.GetFeatureByIdAsync("Plato.Articles")
                .GetAwaiter()
                .GetResult();

            if (feature == null)
            {
                return;
            }

            builder
                .Add(T["Articles"], 2, topics => topics
                    .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                    {
                        ["opts.FeatureId"] = feature.Id,
                        ["opts.Within"] = "Topics"
                    })
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-feature-id", feature.Id}
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                ).Add(T["Article Comments"], 2, f => f
                    .Action("Index", "Home", "Plato.Search", new RouteValueDictionary()
                    {
                        ["opts.FeatureId"] = feature.Id,
                        ["opts.Within"] = "Comments"
                    })
                    .Attributes(new Dictionary<string, object>()
                    {
                        {"data-feature-id", feature.Id}
                    })
                    //.Permission(Permissions.ManageRoles)
                    .LocalNav()
                );
        }
    }

}
