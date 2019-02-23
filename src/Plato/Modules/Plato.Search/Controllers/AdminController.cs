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
using Plato.Search.Services;
using Plato.Search.ViewModels;

namespace Plato.Search.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {
        
        private readonly IViewProviderManager<SearchSettings> _viewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IFullTextCatalogManager<FullTextCatalog> _fullTextCatalogManager;
        private readonly IFullTextIndexManager<FullTextIndex> _fullTextIndexManager;

        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public AdminController(
            IHtmlLocalizer<AdminController> htmlLocalizer,
            IStringLocalizer<AdminController> stringLocalizer,
            IAuthorizationService authorizationService,
            IViewProviderManager<SearchSettings> viewProvider,
            IBreadCrumbManager breadCrumbManager,
            IFullTextCatalogManager<FullTextCatalog> fullTextCatalogManager,
            IFullTextIndexManager<FullTextIndex> fullTextIndexManager,
            IAlerter alerter)
        {
       
            _breadCrumbManager = breadCrumbManager;
            _authorizationService = authorizationService;
            _viewProvider = viewProvider;
            _fullTextCatalogManager = fullTextCatalogManager;
            _fullTextIndexManager = fullTextIndexManager;
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

        public async Task<IActionResult> DeleteCatalog()
        {

            var result = await _fullTextCatalogManager.DeleteAsync(new FullTextCatalog()
            {
                Name = "PlatoCatalog"
            });

            if (result.Succeeded)
            {
                _alerter.Success(T["Settings Updated Successfully!"]);
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

        public async Task<IActionResult> DeleteIndex()
        {

            var result = await _fullTextIndexManager.DeleteAsync(new FullTextIndex()
            {
                TableName = "PlatoCatalog"
            });

            if (result.Succeeded)
            {
                _alerter.Success(T["Settings Updated Successfully!"]);
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
