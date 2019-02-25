using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Features.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Features.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<FeaturesViewModel> _featuresIndexViewProvider;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IAlerter _alerter;
        private readonly IBreadCrumbManager _breadCrumbManager;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IShellFeatureManager shellFeatureManager,
            IShellDescriptorManager shellDescriptorManager,
            IAlerter alerter,
            IViewProviderManager<FeaturesViewModel> featuresIndexViewProvider, 
            IBreadCrumbManager breadCrumbManager)
        {
            _shellFeatureManager = shellFeatureManager;
            _shellDescriptorManager = shellDescriptorManager;
            _alerter = alerter;
            _featuresIndexViewProvider = featuresIndexViewProvider;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }
        
        public async Task<IActionResult> Index()
        {

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Features"]);
            });
            
            var features = await _shellDescriptorManager.GetFeaturesAsync();
            
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
            
            var contexts = await _shellFeatureManager.EnableFeatureAsync(id);
            foreach (var context in contexts)
            {
                if (context.Errors.Any())
                {
                    foreach (var error in context.Errors)
                    {
                        _alerter.Danger(T[$"{context.Feature.ModuleId} could not be enabled. {error.Key} - {error.Value}"]);
                    }
                }
                else
                {
                    _alerter.Success(T[$"{context.Feature.ModuleId} enabled successfully!"]);
                }
                
            }
            
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public async Task<IActionResult> Disable(string id)
        {

            var contexts = await _shellFeatureManager.DisableFeatureAsync(id);
            foreach (var context in contexts)
            {
                if (context.Errors.Any())
                {
                    foreach (var error in context.Errors)
                    {
                        _alerter.Danger(T[$"{error.Key} could not be disabled. The following error occurred: {error.Value}"]);
                    }
                }
                else
                {
                    _alerter.Success(T[$"{context.Feature.ModuleId} disabled successfully!"]);
                }
                
            }
            
            return RedirectToAction(nameof(Index));

        }
        
    }
}
