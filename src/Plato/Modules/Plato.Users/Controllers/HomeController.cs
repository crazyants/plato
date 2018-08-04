using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<UserProfile> _userViewProvider;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IHtmlLocalizer<HomeController> htmlLocalizer,
            IStringLocalizer<HomeController> stringLocalizer,
            IViewProviderManager<UserProfile> userViewProvider,
            IPlatoUserStore<User> platoUserStore,
            IContextFacade contextFacade, 
            UserManager<User> userManager, 
            IAlerter alerter)
        {
            _userViewProvider = userViewProvider;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _userManager = userManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        public async Task<IActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}

            //// Set breadcrumb
            //_breadCrumbManager.Configure(builder =>
            //{
            //    builder.Add(S["Home"], home => home
            //        .Action("Index", "Admin", "Plato.Admin")
            //        .LocalNav()
            //    ).Add(S["Users"]);
            //});

            // default options
            if (filterOptions == null)
            {
                filterOptions = new FilterOptions();
            }

            // default pager
            if (pagerOptions == null)
            {
                pagerOptions = new PagerOptions();
            }

            this.RouteData.Values.Add("page", pagerOptions.Page);

            // Build view
            var result = await _userViewProvider.ProvideIndexAsync(new UserProfile(), this);

            // Return view
            return View(result);

        }
        
        public async Task<IActionResult> Display(int id)
        {

            var user = id > 0
                ? await _platoUserStore.GetByIdAsync(id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Build view
            var result = await _userViewProvider.ProvideDisplayAsync(new UserProfile()
            {
                Id = user.Id
            }, this);

            // Return view
            return View(result);

        }
        

        public async Task<IActionResult> Edit()
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Build view
            var result = await _userViewProvider.ProvideEditAsync(new UserProfile()
            {
                Id = user.Id
            }, this);

            // Return view
            return View(result);

        }


        [HttpPost]
        [ActionName(nameof(Edit))]
        public async Task<IActionResult> EditPost(EditUserViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var profile = new UserProfile()
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                UserName = model.UserName,
                Email = model.Email
            };

            // Validate model state within all view providers
            if (await _userViewProvider.IsModelStateValid(profile, this))
            {
                var result = await _userViewProvider.ProvideUpdateAsync(profile, this);

                // Ensure modelstate is still valid after view providers have executed
                if (ModelState.IsValid)
                {
                    _alerter.Success(T["Profile Updated Successfully!"]);
                    return RedirectToAction(nameof(Edit));
                }

            }
            
            // if we reach this point some view model validation
            // failed within a view provider, display model state errors
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _alerter.Danger(T[error.ErrorMessage]);
                }
            }

            return await Edit();

        }

    }
}
