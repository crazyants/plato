using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Internal.Features;
using Plato.Features.ViewModels;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Features.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<FeaturesViewModel> _featuresIndexViewProvider;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IShellDescriptorFeatureManager _shellDescriptorFeatureManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> localizer,
            IShellFeatureManager shellFeatureManager,
            IShellDescriptorFeatureManager shellDescriptorFeatureManager,
            IAlerter alerter, IViewProviderManager<FeaturesViewModel> featuresIndexViewProvider)
        {
            _shellFeatureManager = shellFeatureManager;
            _shellDescriptorFeatureManager = shellDescriptorFeatureManager;
            _alerter = alerter;
            _featuresIndexViewProvider = featuresIndexViewProvider;

            T = localizer;
        }
        
        public async Task<IActionResult> Index()
        {
            
            var features = await _shellDescriptorFeatureManager.GetFeaturesAsync();
            
            //var enabledFeatures = _shellFEatureManager.

            var model = new FeaturesViewModel()
            {
                Features = features
            };
            

            var result = await _featuresIndexViewProvider.ProvideIndexAsync(model, this);
            return View(result);

            
        }

        [HttpPost]
        public async Task<IActionResult> Enable(string id)
        {
            
            var results = await _shellFeatureManager.EnableFeaturesAsync(new string[] {id});

            _alerter.Success(T["Feature enabled successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Disable(string id)
        {

            var results = await _shellFeatureManager.DisableFeaturesAsync(new string[] { id });

            _alerter.Success(T["Feature disabled successfully!"]);

            return RedirectToAction(nameof(Index));

        }




    }
}
