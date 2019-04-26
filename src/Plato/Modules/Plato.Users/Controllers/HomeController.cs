using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EditProfileViewModel> _editProfileViewProvider;
        private readonly IViewProviderManager<EditAccountViewModel> _editAccountViewProvider;
        private readonly IViewProviderManager<EditSettingsViewModel> _editSettingsViewProvider;
        private readonly IViewProviderManager<EditSignatureViewModel> _editSignatureViewProvider;
        private readonly IViewProviderManager<Profile> _viewProvider;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IUserEmails _userEmails;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<EditProfileViewModel> editProfileViewProvider,
            IViewProviderManager<EditAccountViewModel> editAccountViewProvider,
            IViewProviderManager<EditSettingsViewModel> editSettingsViewProvider,
            IViewProviderManager<EditSignatureViewModel> editSignatureViewProvider,
            IViewProviderManager<Profile> viewProvider,
            IPlatoUserManager<User> platoUserManager,
            IPlatoUserStore<User> platoUserStore,
            IBreadCrumbManager breadCrumbManager,
            ITimeZoneProvider timeZoneProvider,
            IPageTitleBuilder pageTitleBuilder,
            UserManager<User> userManager,
            IContextFacade contextFacade,
            IUserEmails userEmails,
            IAlerter alerter)
        {

            _editProfileViewProvider = editProfileViewProvider;
            _editAccountViewProvider = editAccountViewProvider;
            _editSettingsViewProvider = editSettingsViewProvider;
            _editSignatureViewProvider = editSignatureViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _timeZoneProvider = timeZoneProvider;
            _platoUserManager = platoUserManager;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _userManager = userManager;
            _alerter = alerter;
            _pageTitleBuilder = pageTitleBuilder;

            _userEmails = userEmails;
            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #region "Actions"

        // -----------------
        // User List
        // -----------------

        public async Task<IActionResult> Index(UserIndexOptions opts, PagerOptions pager)
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
            if (pager.Size != defaultPagerOptions.Size)
                this.RouteData.Values.Add("pager.size", pager.Size);

            // Build view model
            var viewModel = new UserIndexViewModel()
            {
                Options = opts,
                Pager = pager
            };

            // Add view model to context
            HttpContext.Items[typeof(UserIndexViewModel)] = viewModel;
            
            // If we have a pager.page querystring value return paged results
            if (int.TryParse(HttpContext.Request.Query["pager.page"], out var page))
            {
                if (page > 0)
                    return View("GetUsers", viewModel);
            }
            
            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Users"]);
            });
            
            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideIndexAsync(new Profile(), this));

        }

        // -----------------
        // User Profile
        // -----------------

        public async Task<IActionResult> Display(DisplayUserOptions opts)
        {

            var user = opts.Id > 0
                ? await _platoUserStore.GetByIdAsync(opts.Id)
                : await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            
            // Build page title
            _pageTitleBuilder.AddSegment(S[user.DisplayName], int.MaxValue);

            // Build breadcrumb
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

            // Add user to context
            HttpContext.Items[typeof(User)] = user;
            
            // Build view model
            var viewModel = new Profile()
            {
                Id = user.Id
            };

            // Return view
            return View((LayoutViewModel) await _viewProvider.ProvideDisplayAsync(viewModel, this));

        }

        // -----------------
        // Edit Profile
        // -----------------

        public async Task<IActionResult> EditProfile()
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure user is authenticated
            if (user == null)
            {
                return Unauthorized();
            }
            
            // Meta data
            var data = user.GetOrCreate<UserDetail>();

            // Build view model
            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Location = data.Profile.Location,
                Bio = data.Profile.Bio,
                Url = data.Profile.Url,
                Avatar = user.Avatar
            };

            // Return view
            return View((LayoutViewModel) await _editProfileViewProvider.ProvideEditAsync(editProfileViewModel, this));

        }

        [HttpPost, ActionName(nameof(EditProfile))]
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

                // Ensure model state is still valid after view providers have executed
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

        // -----------------
        // Edit Account
        // -----------------

        public async Task<IActionResult> EditAccount()
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure user is authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Build view model
            var viewModel = new EditAccountViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            
            // Return view
            return View((LayoutViewModel) await _editAccountViewProvider.ProvideEditAsync(viewModel, this));

        }

        [HttpPost, ActionName(nameof(EditAccount)), ValidateAntiForgeryToken]
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

                // Ensure model state is still valid after view providers have executed
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
        
        public async Task<IActionResult> ResetPassword()
        {

            // Get user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure user exists
            if (user == null)
            {
                return NotFound();
            }

            var result = await _platoUserManager.GetForgotPasswordUserAsync(user.UserName);
            if (result.Succeeded)
            {
                // Ensure account has been confirmed
                if (await _userManager.IsEmailConfirmedAsync(user))
                {
                    user.ResetToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Response.ResetToken));
                    var emailResult = await _userEmails.SendPasswordResetTokenAsync(result.Response);
                    if (emailResult.Succeeded)
                    {
                        _alerter.Success(T["Check your email. We've sent you a password reset link!"]);
                    }
                    else
                    {
                        foreach (var error in emailResult.Errors)
                        {
                            _alerter.Danger(T[error.Description]);
                            //ViewData.ModelState.AddModelError(string.Empty, error.Description);
                        }}
                }
                else
                {
                    _alerter.Danger(T["You must confirm your email before you can reset your password!"]);
                    //ViewData.ModelState.AddModelError(string.Empty, "You must confirm your email before you can reset your password!");
                }
                 
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                    //ViewData.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction(nameof(EditAccount));

        }

        // -----------------
        // Edit Settings
        // -----------------

        public async Task<IActionResult> EditSettings()
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure user is authenticated
            if (user == null)
            {
                return Unauthorized();
            }
            
            // Get user data
            var data = user.GetOrCreate<UserDetail>();

            // Build view model
            var viewModel = new EditSettingsViewModel()
            {
                Id = user.Id,
                TimeZone = user.TimeZone,
                ObserveDst = user.ObserveDst,
                Culture = user.Culture,
                AvailableTimeZones = await GetAvailableTimeZonesAsync()
            };

            // Return view
            return View((LayoutViewModel) await _editSettingsViewProvider.ProvideEditAsync(viewModel, this));

        }

        [HttpPost, ActionName(nameof(EditSettings)), ValidateAntiForgeryToken]
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

                // Ensure model state is still valid after view providers have executed
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

        // -----------------
        // Edit Signature
        // -----------------

        public async Task<IActionResult> EditSignature()
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Ensure user is authenticated
            if (user == null)
            {
                return Unauthorized();
            }

            // Get user data
            var data = user.GetOrCreate<UserDetail>();

            // Build view model
            var viewModel = new EditSignatureViewModel()
            {
                Id = user.Id,
                Signature = user.Signature
            };

            // Return view
            return View((LayoutViewModel) await _editSignatureViewProvider.ProvideEditAsync(viewModel, this));

        }

        [HttpPost, ActionName(nameof(EditSignature)), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSignaturePost(EditSignatureViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _editSignatureViewProvider.IsModelStateValid(model, this))
            {

                await _editSignatureViewProvider.ProvideUpdateAsync(model, this);

                // Ensure model state is still valid after view providers have executed
                if (ModelState.IsValid)
                {
                    _alerter.Success(T["Signature Updated Successfully!"]);
                    return RedirectToAction(nameof(EditSignature));
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

            return await EditSignature();

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
