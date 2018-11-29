using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;

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

        public void BuildNavigation(string name, NavigationBuilder builder)
        {

            if (!String.Equals(name, "search", StringComparison.OrdinalIgnoreCase))
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
                .Add(T["Discuss"], 2, discuss => discuss
                    .Add(T["Topics"], 1, topics => topics
                        .Attributes(new Dictionary<string, object>()
                        {
                            { "data-feature-id", feature.Id }
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ).Add(T["Replies"], 2, favourites => favourites
                        .Attributes(new Dictionary<string, object>()
                        {
                            { "data-feature-id", feature.Id }
                        })
                        //.Permission(Permissions.ManageRoles)
                        .LocalNav()
                    ), new List<string>() { "discuss", "text-hidden" });
        }
    }

}
