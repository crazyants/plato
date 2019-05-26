using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Features.ViewComponents
{

    public class FeatureUpdateListViewComponent : ViewComponent
    {

        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IShellDescriptorStore _shellDescriptorStore;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public FeatureUpdateListViewComponent(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IShellDescriptorManager shellDescriptorManager,
            IShellDescriptorStore shellDescriptorStore)
        {

            _shellDescriptorManager = shellDescriptorManager;
            _shellDescriptorStore = shellDescriptorStore;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IViewComponentResult> InvokeAsync(FeatureIndexOptions options)
        {

            // Get all features present within /Modules directory
            var features = await _shellDescriptorManager.GetFeaturesAsync();

            // Get all enabled features from database
            var descriptor = await _shellDescriptorStore.GetAsync();

            // Build a list of out of date features

            foreach (var feature in features)
            {

                var descriptorFeature = descriptor.Modules.FirstOrDefault(m => m.ModuleId.Equals(feature.ModuleId));

                var featureVersion = feature.Descriptor.Version;

                var installedVersion = descriptorFeature?.Version;


                var availableVersion = featureVersion.ToVersion();

            }



            // No features
            if (features == null)
            {
                return View(new FeatureUpdatesIndexViewModel()
                {
                    Options = options
                });
            }
            
            if (options.HideEnabled)
            {
                features = features.Where(f => f.IsEnabled == false);
            }

            return View(new FeatureUpdatesIndexViewModel()
            {
                Options = options,
                Features = features
            });
        }
        
    }

}
