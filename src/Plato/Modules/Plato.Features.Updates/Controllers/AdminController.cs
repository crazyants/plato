using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Features.Updates.ViewModels;
using Plato.Internal.Features.Abstractions;
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
        
        private readonly IViewProviderManager<FeatureUpdatesIndexViewModel> _viewProvider;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IShellFeatureManager _shellFeatureManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IShellFeatureManager shellFeatureManager,
            IViewProviderManager<FeatureUpdatesIndexViewModel> viewProvider, 
            IBreadCrumbManager breadCrumbManager,
            IAuthorizationService authorizationService,
            IAlerter alerter,
            IShellDescriptorManager shellDescriptorManager)
        {
            _shellFeatureManager = shellFeatureManager;
            _viewProvider = viewProvider;
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _alerter = alerter;
            _shellDescriptorManager = shellDescriptorManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }
        
        public async Task<IActionResult> Index(FeatureIndexOptions opts)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageFeatureUpdates))
            {
                return Unauthorized();
            }
            
            if (opts == null)
            {
                opts = new FeatureIndexOptions();
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
  
            var model = new FeatureUpdatesIndexViewModel()
            {
                Options = opts
            };
            
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(model, this));
            
        }
        
        public async Task<IActionResult> Enable(
            string id, 
            string category,
            string returnUrl)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.UpdateFeatures))
            {
                return Unauthorized();
            }

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

            if (!string.IsNullOrEmpty(returnUrl))
            {
                // Redirect to returnUrl
                return RedirectToLocal(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(Index), new RouteValueDictionary()
                {
                    ["opts.category"] = category
                });
            }


        }
        
        IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }

        async Task<string> GetCategoriesNameAsync(string id)
        {
        
            var features = await _shellDescriptorManager.GetFeaturesAsync();
            foreach (var feature in features
                .GroupBy(f => f.Descriptor.Category)
                .OrderBy(o => o.Key))
            {
                if (feature.Key.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return feature.Key;
                }
            }

            return string.Empty;

        }
        
    }

}
