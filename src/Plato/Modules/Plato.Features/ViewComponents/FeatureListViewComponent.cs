using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Features.ViewModels;
using Plato.Internal.Features.Abstractions;

namespace Plato.Features.ViewComponents
{

    public class FeatureListViewComponent : ViewComponent
    {

        private readonly IShellDescriptorManager _shellDescriptorManager;

        public FeatureListViewComponent(
            IShellDescriptorManager shellDescriptorManager)
        {
            _shellDescriptorManager = shellDescriptorManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string category,
            bool showEnabled = true)
        {

            // Get features
            var features = await _shellDescriptorManager.GetFeaturesAsync();

            // No features
            if (features == null)
            {
                return View(new FeaturesViewModel());
            }

            // Filter features by category
            if (!string.IsNullOrEmpty(category))
            {
                features = features.Where(f => f.Descriptor?.Category != null && f.Descriptor.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!showEnabled)
            {
                features = features.Where(f => f.IsEnabled == false);
            }

            return View(new FeaturesViewModel()
            {
                Features = features
            });
        }
        
    }

}
