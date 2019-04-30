using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;
using Plato.Search.Models;
using Plato.Search.Services;
using Plato.Search.ViewModels;
using Plato.Internal.Security.Abstractions;

namespace Plato.Search.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<SearchSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFullTextCatalogManager _fullTextCatalogManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IViewProviderManager<SearchSettings> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IFullTextCatalogManager fullTextCatalogManager,
            IAlerter alerter)
        {
       
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _fullTextCatalogManager = fullTextCatalogManager;
            _viewProvider = viewProvider;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }
        
        public async Task<IActionResult> Index()
        {
            
            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSearchSettings))
            {
                return Unauthorized();
            }

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

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideEditAsync(new SearchSettings(), this));

        }
        
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Index))]
        public async Task<IActionResult> IndexPost(SearchSettingsViewModel viewModel)
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSearchSettings))
            {
                return Unauthorized();
            }
            
            // Execute view providers ProvideUpdateAsync method
            await _viewProvider.ProvideUpdateAsync(new SearchSettings(), this);

            // Add alert
            _alerter.Success(T["Settings Updated Successfully!"]);

            return RedirectToAction(nameof(Index));

        }
        
        public async Task<IActionResult> CreateCatalog()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSearchSettings))
            {
                return Unauthorized();
            }
            
            // Create catalog
            var result = await _fullTextCatalogManager.CreateCatalogAsync();
            if (result.Succeeded)
            {
                _alerter.Success(T["Catalog Created Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            
            return RedirectToAction(nameof(Index));

        }
        
        public async Task<IActionResult> DeleteCatalog()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSearchSettings))
            {
                return Unauthorized();
            }
            
            // Delete catalog
            var result = await _fullTextCatalogManager.DropCatalogAsync();
            if (result.Succeeded)
            {
                _alerter.Success(T["Catalog Deleted Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> RebuildCatalog()
        {

            // Ensure we have permission
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageSearchSettings))
            {
                return Unauthorized();
            }
            
            // Rebuild catalog
            var result = await _fullTextCatalogManager.RebuildCatalogAsync();
            if (result.Succeeded)
            {
                _alerter.Success(T["Catalog Rebuilt Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            
            return RedirectToAction(nameof(Index));

        }
        
    }

}
