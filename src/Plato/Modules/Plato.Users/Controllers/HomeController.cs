using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Migrations;
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
using Plato.Users.Services;
using Plato.Users.ViewModels;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EditProfileViewModel> _editProfileViewProvider;
        private readonly IViewProviderManager<EditAccountViewModel> _editAccountViewProvider;
        private readonly IViewProviderManager<EditSettingsViewModel> _editSettingsViewProvider;
        private readonly IViewProviderManager<EditSignatureViewModel> _editSignatureViewProvider;
        private readonly IAuthorizationService _authorizationService;
        private readonly IViewProviderManager<Profile> _viewProvider;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IPageTitleBuilder _pageTitleBuilder;
        private readonly ITimeZoneProvider _timeZoneProvider;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IUserEmails _userEmails;
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
            IAuthorizationService authorizationService,
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
            _authorizationService = authorizationService;
            _breadCrumbManager = breadCrumbManager;
            _pageTitleBuilder = pageTitleBuilder;
            _timeZoneProvider = timeZoneProvider;
            _platoUserManager = platoUserManager;
            _platoUserStore = platoUserStore;
            _contextFacade = contextFacade;
            _viewProvider = viewProvider;
            _userManager = userManager;
            _userEmails = userEmails;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;
        }

        #region "Actions"

        // -----------------
        // User List
        // -----------------

        public async Task<IActionResult> Index(UserIndexOptions opts, PagerOptions pager)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.ViewUsers))
            {
                return Unauthorized();
            }

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
        // Display User
        // -----------------

        public async Task<IActionResult> Display(DisplayUserOptions opts)
        {

            // Ensure we have permission 
            if (!await _authorizationService.AuthorizeAsync(User,
                Permissions.ViewProfiles))
            {
                return Unauthorized();
            }
            
            // Get user to display
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
        // Get User
        // -----------------
        
        public async Task<IActionResult> GetUser(DisplayUserOptions opts)
        {
            
            // Ensure we have defaults

            if (opts == null)
            {
                opts = new DisplayUserOptions();
            }

            if (opts.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(opts.Id));
            }

            // Get user
            var user = await _platoUserStore.GetByIdAsync(opts.Id);
             
            // Ensure user exists
            if (user == null)
            {
                return NotFound();
            }

            // Return view
            return View(user);

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

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Your Account"]);
            });

            // Build view model
            var editProfileViewModel = new EditProfileViewModel()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Location = user.Location,
                Biography = user.Biography,
                Url = user.Url,
                Avatar = user.Avatar
            };

            // Return view
            return View((LayoutViewModel) await _editProfileViewProvider.ProvideEditAsync(editProfileViewModel, this));

        }
    
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditProfile))]
        public async Task<IActionResult> EditProfilePost(EditProfileViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
            {
                return NotFound();
            }
            
            // Validate model state within all view providers
            if (await _editProfileViewProvider.IsModelStateValidAsync(model, this))
            {

                user.DisplayName = model.DisplayName;
                user.Location = model.Location;
                user.Url = model.Url;
                user.Biography = model.Biography;

                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    // Invoke BuildUpdateAsync within involved view providers
                    await _editProfileViewProvider.ProvideUpdateAsync(model, this);

                    // Ensure model state is still valid after view providers have executed
                    if (ModelState.IsValid)
                    {
                        // Add confirmation
                        _alerter.Success(T["Profile Updated Successfully!"]);
                        // Redirect
                        return RedirectToAction(nameof(EditProfile));
                    }

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            
            // If errors occur manually expire the cache otherwise our
            // modifications made above to the object may persist as the
            // object is not updated and the cache is not invalidated by the store
            _platoUserStore.CancelTokens(user);
            
            // Display errors
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

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Your Account"]);
            });

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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditAccount))]
        public async Task<IActionResult> EditAccountPost(EditAccountViewModel model)
        {

            // Get user
            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            // Ensure user exists
            if (user == null)
            {
                return NotFound();
            }

            // Get composed model from involved view providers
            model = await _editAccountViewProvider.ComposeModelAsync(model, this);

            // Validate model state within all view providers
            if (await _editAccountViewProvider.IsModelStateValidAsync(model, this))
            {
                
                // Flags to indicate if the username or email address have changed
                var emailChanged = model.Email != null && !model.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase);
                var usernameChanged = model.UserName != null && !model.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

                // Update user
                user.UserName = model.UserName;
                user.Email = model.Email;
                
                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    
                    if (emailChanged)
                    {
                        // Only call SetEmailAsync if the email address changes
                        // SetEmailAsync internally sets EmailConfirmed to "false"
                        await _userManager.SetEmailAsync(user, model.Email);
                    }

                    if (usernameChanged)
                    {
                        // SetUserNameAsync internally sets a new SecurityStamp
                        // which will invalidate the authentication cookie
                        // This will force the user to be logged out
                        await _userManager.SetUserNameAsync(user, model.UserName);
                    }

                    // Invoke BuildUpdateAsync within involved view providers
                    await _editAccountViewProvider.ProvideUpdateAsync(model, this);

                    // Ensure model state is still valid after view providers have executed
                    if (ModelState.IsValid)
                    {
                        // Add confirmation
                        _alerter.Success(T["Account Updated Successfully!"]);
                        // Redirect back
                        return RedirectToAction(nameof(EditAccount));
                    }

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            // If errors occur manually expire the cache otherwise our
            // modifications made above to the object may persist as the
            // object is not updated and the cache is not invalidated by the store
            _platoUserStore.CancelTokens(user);

            // Display errors
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
                        }
                    }
                }
                else
                {
                    _alerter.Danger(T["You must confirm your email before you can reset your password!"]);
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

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Your Account"]);
            });

            // Build view model
            var viewModel = new EditSignatureViewModel()
            {
                Id = user.Id,
                Signature = user.Signature
            };

            // Return view
            return View((LayoutViewModel) await _editSignatureViewProvider.ProvideEditAsync(viewModel, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditSignature))]
        public async Task<IActionResult> EditSignaturePost(EditSignatureViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _editSignatureViewProvider.IsModelStateValidAsync(model, this))
            {

                // Update user
                user.Signature = model.Signature;

                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    // Invoke BuildUpdateAsync within involved view providers
                    await _editSignatureViewProvider.ProvideUpdateAsync(model, this);

                    // Ensure model state is still valid after view providers have executed
                    if (ModelState.IsValid)
                    {
                        // Add confirmation
                        _alerter.Success(T["Signature Updated Successfully!"]);
                        // Redirect
                        return RedirectToAction(nameof(EditSignature));
                    }

                }
                else
                {
                    // Errors that may have occurred whilst updating the entity
                    foreach (var error in result.Errors)
                    {
                        ViewData.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If errors occur manually expire the cache otherwise our
            // modifications made above to the object may persist as the
            // object is not updated and the cache is not invalidated by the store
            _platoUserStore.CancelTokens(user);

            // Display errors
            return await EditSignature();

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

            // Breadcrumb
            _breadCrumbManager.Configure(builder =>
            {
                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Your Account"]);
            });

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

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditSettings))]
        public async Task<IActionResult> EditSettingsPost(EditSettingsViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Validate model state within all view providers
            if (await _editSettingsViewProvider.IsModelStateValidAsync(model, this))
            {

                // Update user
                user.TimeZone = model.TimeZone;
                user.ObserveDst = model.ObserveDst;
                user.Culture = model.Culture;
                
                // Update user
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    // Invoke BuildUpdateAsync within involved view providers
                    await _editSettingsViewProvider.ProvideUpdateAsync(model, this);

                    // Ensure model state is still valid after view providers have executed
                    if (ModelState.IsValid)
                    {
                        // Add confirmation
                        _alerter.Success(T["Settings Updated Successfully!"]);
                        // Redirect
                        return RedirectToAction(nameof(EditSettings));
                    }
                }

            }
            
            // If errors occur manually expire the cache otherwise our
            // modifications made above to the object may persist as the
            // object is not updated and the cache is not invalidated by the store
            _platoUserStore.CancelTokens(user);

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
