using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Features.Updates.Services;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Features.Updates.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IShellFeatureUpdater _shellFeatureUpdater;

        private readonly IViewProviderManager<FeatureUpdatesViewModel> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<FeatureUpdatesViewModel> viewProvider, 
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IShellFeatureUpdater shellFeatureUpdater,
            IAlerter alerter)
        {

            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _shellFeatureUpdater = shellFeatureUpdater;
            _viewProvider = viewProvider;
            _alerter = alerter;
    
            T = htmlLocalizer;
            S = stringLocalizer;
        }
        
        public async Task<IActionResult> Index(FeatureUpdateOptions opts)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageFeatureUpdates))
            {
                return Unauthorized();
            }
            
            if (opts == null)
            {
                opts = new FeatureUpdateOptions();
            }

            // Build breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder
                    .Add(S["Home"], home => home
                        .Action("Index", "Admin", "Plato.Admin")
                        .LocalNav())
                    .Add(S["Features"], features => features
                        .Action("Index", "Admin", "Plato.Features")
                        .LocalNav())
                    .Add(S["Updates"]);
            });
  
            var model = new FeatureUpdatesViewModel()
            {
                Options = opts
            };
            
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(model, this));
            
        }

        public async Task<IActionResult> Update(string id)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.UpdateFeatures))
            {
                return Unauthorized();
            }

            var result = await _shellFeatureUpdater.UpdateAsync(id);
            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            else
            {
                _alerter.Success(T[$"{id} Updated Successfully!"]);
            }
            
            return RedirectToAction(nameof(Index));
            
        }
        
    }

}
