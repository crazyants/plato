using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;
using TimeZone = Plato.Internal.Localization.Abstractions.TimeZone;

namespace Plato.Users.Controllers
{
    public class ProfileController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EditProfileViewModel> _editProfileViewProvider;
        private readonly IViewProviderManager<EditAccountViewModel> _editAccountViewProvider;
        private readonly IViewProviderManager<EditSettingsViewModel> _editSettingsViewProvider;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;
        private readonly ITimeZoneProvider<TimeZone> _timeZoneProvider;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public ProfileController(
            IViewProviderManager<EditProfileViewModel> editProfileViewProvider,
            IViewProviderManager<EditAccountViewModel> editAccountViewProvider,
            IViewProviderManager<EditSettingsViewModel> editSettingsViewProvider,
            IHtmlLocalizer<HomeController> htmlLocalizer,
            IStringLocalizer<HomeController> stringLocalizer,
            ITimeZoneProvider<TimeZone> timeZoneProvider,
            IPlatoUserStore<User> platoUserStore,
            IContextFacade contextFacade, 
            UserManager<User> userManager, 
            IAlerter alerter
           )
        {

            _editProfileViewProvider = editProfileViewProvider;
            _editAccountViewProvider = editAccountViewProvider;
            _editSettingsViewProvider = editSettingsViewProvider;
            _platoUserStore = platoUserStore;
            _timeZoneProvider = timeZoneProvider;
            _contextFacade = contextFacade;
            _userManager = userManager;
            _alerter = alerter;
            

            T = htmlLocalizer;
            S = stringLocalizer;
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
                Url = data.Profile.Url
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
            
            // Get available timezones
            var availableTimeZones = await _timeZoneProvider.GetTimeZones();

            // Convert timezones to dictionary for view model
            var timeZones = availableTimeZones
                .OrderBy(tz => tz.UtcOffSet)
                .ToDictionary(
                    tz => tz.Name.ToString() + tz.Region.ToString(),
                    tz => tz.UtcOffSet,
                    StringComparer.OrdinalIgnoreCase);

            // Build view
            var result = await _editSettingsViewProvider.ProvideEditAsync(new EditSettingsViewModel()
            {
                Id = user.Id,
                TimeZoneOffSet = data.Settings.TimeZoneOffset,
                Culture = data.Settings.Culture,
                AvailableTimeZones = timeZones
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
        
    }
}
