using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Features.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Features.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<FeaturesViewModel> _viewProvider;
        //private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IShellFeatureManager shellFeatureManager,
            IShellDescriptorManager shellDescriptorManager,
            IViewProviderManager<FeaturesViewModel> viewProvider, 
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
            _shellFeatureManager = shellFeatureManager;
            _viewProvider = viewProvider;
            //_shellDescriptorManager = shellDescriptorManager;
            _breadCrumbManager = breadCrumbManager;
            _alerter = alerter;

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
            
            //var features = await _shellDescriptorManager.GetFeaturesAsync();
            
            //var enabledFeatures = _shellFEatureManager.

            var model = new FeaturesViewModel();
            
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(model, this));
            
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
