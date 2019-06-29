using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Facebook.Models;
using Plato.Facebook.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Facebook.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IViewProviderManager<FacebookSettings> _viewProvider;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IViewProviderManager<FacebookSettings> viewProvider,
            IAuthorizationService authorizationService,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
       
            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditFacebookSettings))
            {
                return Unauthorized();
            }
            
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], channels => channels
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Facebook"]);
            });

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new FacebookSettings(), this));
            
        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(FacebookSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditFacebookSettings))
            {
                return Unauthorized();
            }

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new FacebookSettings(), this);
        
            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);
      
            return RedirectToAction(nameof(Index));
            
        }
      
    }

}
