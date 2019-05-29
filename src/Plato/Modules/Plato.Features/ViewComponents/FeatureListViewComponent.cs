using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Features.ViewModels;
using Plato.Internal.Features.Abstractions;

namespace Plato.Features.ViewComponents
{

    public class FeatureListViewComponent : ViewComponent
    {

        private readonly IShellDescriptorManager _shellDescriptorManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public FeatureListViewComponent(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IShellDescriptorManager shellDescriptorManager)
        {

            _shellDescriptorManager = shellDescriptorManager;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IViewComponentResult> InvokeAsync(FeatureIndexOptions options)
        {

            // Get features
            var features = await _shellDescriptorManager.GetFeaturesAsync();

            // No features
            if (features == null)
            {
                return View(new FeaturesIndexViewModel()
                {
                    Options = options,
                    AvailableCategories = await GetAvailableFeatureCategoriesAsync()
                });
            }

            // Filter features by category
            if (!string.IsNullOrEmpty(options.Category))
            {
                // Don't apply filter if all categories are selected
                if (!options.Category.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    features = features.Where(f => f.Descriptor?.Category != null && f.Descriptor.Category.Equals(options.Category, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (options.HideEnabled)
            {
                features = features.Where(f => f.IsEnabled == false);
            }

            return View(new FeaturesIndexViewModel()
            {
                Options = options,
                AvailableCategories = await GetAvailableFeatureCategoriesAsync(),
                Features = features
            });
        }

        async Task<IEnumerable<SelectListItem>> GetAvailableFeatureCategoriesAsync()
        {

            var output = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Text = S["All"],
                    Value = "all"
                }
            };

            var features = await _shellDescriptorManager.GetFeaturesAsync();
            foreach (var feature in features.GroupBy(f => f.Descriptor.Category).OrderBy(o => o.Key))
            {
                output.Add(new SelectListItem
                {
                    Text = feature.Key,
                    Value = feature.Key.ToLower()
                });
            }

            return output;
        }
        
    }

}
