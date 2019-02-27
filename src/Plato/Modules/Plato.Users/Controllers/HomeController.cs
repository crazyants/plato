using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EditProfileViewModel> _editProfileViewProvider;
        private readonly IViewProviderManager<EditAccountViewModel> _editAccountViewProvider;
        private readonly IViewProviderManager<EditSettingsViewModel> _editSettingsViewProvider;
        private readonly IViewProviderManager<UserProfile> _viewProvider;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;
        private readonly ITimeZoneProvider _timeZoneProvider;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<UserProfile> viewProvider,
            IPlatoUserStore<User> platoUserStore,
            IContextFacade contextFacade, 
            UserManager<User> userManager, 
            IAlerter alerter,
            ITimeZoneProvider timeZoneProvider,
            IViewProviderManager<EditProfileViewModel> editProfileViewProvider,
            IViewProviderManager<EditAccountViewModel> editAccountViewProvider,
            IViewProviderManager<EditSettingsViewModel> editSettingsViewProvider,
            IBreadCrumbManager breadCrumbManager)
        {
            _viewProvider = viewProvider;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _userManager = userManager;
            _alerter = alerter;
            _timeZoneProvider = timeZoneProvider;
            _editProfileViewProvider = editProfileViewProvider;
            _editAccountViewProvider = editAccountViewProvider;
            _editSettingsViewProvider = editSettingsViewProvider;
            _breadCrumbManager = breadCrumbManager;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #region "Actions"

        // User List
        // --------------------------

        public async Task<IActionResult> Index(
            int offset,
            UserIndexOptions opts,
            PagerOptions pager)
        {

            //if (!await _authorizationService.AuthorizeAsync(User, PermissionsProvider.ManageRoles))
            //{
            //    return Unauthorized();
            //}
        
            // default options
            if (opts == null)
            {
                opts = new UserIndexOptions();
            }

            // default pager
            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Set pager call back Url
            pager.Url = _contextFacade.GetRouteUrl(pager.Route(RouteData));

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"]);
            });

            // Get default options
            var defaultViewOptions = new UserIndexOptions();
            var defaultPagerOptions = new PagerOptions();

            // Add non default route data for pagination purposes
            if (opts.Search != defaultViewOptions.Search)
                this.RouteData.Values.Add("opts.search", opts.Search);
            if (opts.Sort != defaultViewOptions.Sort)
                this.RouteData.Values.Add("opts.sort", opts.Sort);
            if (opts.Order != defaultViewOptions.Order)
                this.RouteData.Values.Add("opts.order", opts.Order);
            if (pager.Page != defaultPagerOptions.Page)
                this.RouteData.Values.Add("pager.page", pager.Page);
            if (pager.PageSize != defaultPagerOptions.PageSize)
                this.RouteData.Values.Add("pager.size", pager.PageSize);

            // Build view model
            var viewModel = new UserIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view options to context for use within view adaptors
            HttpContext.Items[typeof(UserIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetUsers", viewModel);
            }
            
            // Return view
            return View(await _viewProvider.ProvideIndexAsync(new UserProfile(), this));

        }

        // User Profile
        // --------------------------

        public async Task<IActionResult> Display(int id)
        {

            var user = id > 0
                ? await _platoUserStore.GetByIdAsync(id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"], discuss => discuss
                    .Action("Index", "Home", "Plato.Users")
                    .LocalNav()
                ).Add(S[user.DisplayName]);
            });

            // Build view
            var result = await _viewProvider.ProvideDisplayAsync(new UserProfile()
            {
                Id = user.Id
            }, this);

            // Return view
            return View(result);

        }

        // Edit Profile
        // --------------------------
        
        public async Task<IActionResult> EditProfile()
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var data = user.GetOrCreate<UserDetail>();

            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Location = data.Profile.Location,
                Bio = data.Profile.Bio,
                Url = data.Profile.Url,
                Avatar = user.Avatar
            };

            // Build view
            var result = await _editProfileViewProvider.ProvideEditAsync(editProfileViewModel, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ActionName(nameof(EditProfile))]
        public async Task<IActionResult> EditProfilePost(EditProfileViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }


            // Validate model state within all view providers
            if (await _editProfileViewProvider.IsModelStateValid(model, this))
            {
                var result = await _editProfileViewProvider.ProvideUpdateAsync(model, this);

                // Ensure modelstate is still valid after view providers have executed
                if (ModelState.IsValid)
                {
                    _alerter.Success(T["Profile Updated Successfully!"]);
                    return RedirectToAction(nameof(EditProfile));
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

            return await EditProfile();

        }

        // Edit Account
        // --------------------------

        public async Task<IActionResult> EditAccount()
        {
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new EditAccountViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            // Build view
            var result = await _editAccountViewProvider.ProvideEditAsync(viewModel, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ActionName(nameof(EditAccount))]
        public async Task<IActionResult> EditAccountPost(EditAccountViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _editAccountViewProvider.IsModelStateValid(model, this))
            {
                var result = await _editAccountViewProvider.ProvideUpdateAsync(model, this);

                // Ensure modelstate is still valid after view providers have executed
                if (ModelState.IsValid)
                {
                    _alerter.Success(T["Account Updated Successfully!"]);
                    return RedirectToAction(nameof(EditAccount));
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

            return await EditAccount();

        }

        // Edit Settings
        // --------------------------

        public async Task<IActionResult> EditSettings()
        {
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Get user data
            var data = user.GetOrCreate<UserDetail>();

            // Build view model
            var result = await _editSettingsViewProvider.ProvideEditAsync(new EditSettingsViewModel()
            {
                Id = user.Id,
                TimeZone = user.TimeZone,
                ObserveDst = user.ObserveDst,
                Culture = user.Culture,
                AvailableTimeZones = await GetAvailableTimeZonesAsync()
            }, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ActionName(nameof(EditSettings))]
        public async Task<IActionResult> EditSettingsPost(EditSettingsViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _editSettingsViewProvider.IsModelStateValid(model, this))
            {
                await _editSettingsViewProvider.ProvideUpdateAsync(model, this);

                // Ensure modelstate is still valid after view providers have executed
                if (ModelState.IsValid)
                {
                    _alerter.Success(T["Settings Updated Successfully!"]);
                    return RedirectToAction(nameof(EditSettings));
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

            return await EditSettings();

        }

        #endregion

        #region "Private Methods"
        
        async Task<IEnumerable<SelectListItem>> GetAvailableTimeZonesAsync()
        {
            // Build timezones 
            var timeZones = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["-"],
                    Value = ""
                }
            };
            foreach (var z in await _timeZoneProvider.GetTimeZonesAsync())
            {
                timeZones.Add(new SelectListItem
                {
                    Text = z.DisplayName,
                    Value = z.Id
                });
            }

            return timeZones;
        }
        
        #endregion

    }
}
