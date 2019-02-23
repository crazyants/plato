using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;
using Plato.Internal.Layout.ViewProviders;
using Plato.Search.Models;
using Plato.Search.ViewModels;

namespace Plato.Search.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<SearchSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IViewProviderManager<SearchSettings> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IAlerter alerter)
        {
       
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Admin", "Plato.Admin")
                    .LocalNav()
                ).Add(S["Settings"], settings => settings
                    .Action("Index", "Admin", "Plato.Settings")
                    .LocalNav()
                ).Add(S["Search"]);
            });

            // Build view
            var result = await _viewProvider.ProvideEditAsync(new SearchSettings(), this);

            // Return view
            return View(result);

        }
        
        [HttpPost, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SearchSettingsViewModel viewModel)
        {

            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new SearchSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }

        // ------------

        public Task<IActionResult> DropCatalog()
        {
            return Task.FromResult(default(IActionResult));
        }



    }

}
