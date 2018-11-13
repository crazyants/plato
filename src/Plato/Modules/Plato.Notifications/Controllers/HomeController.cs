using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Notifications.ViewModels;

namespace Plato.Notifications.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<EditNotificationsViewModel> _editProfileViewProvider;
        private readonly IContextFacade _contextFacade;
        private readonly UserManager<User> _userManager;

        public HomeController(
            IContextFacade contextFacade,
            IViewProviderManager<EditNotificationsViewModel> editProfileViewProvider,
            UserManager<User> userManager)
        {
            _contextFacade = contextFacade;
            _editProfileViewProvider = editProfileViewProvider;
            _userManager = userManager;
        }

        public async Task<IActionResult> EditProfile()
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            //var data = user.GetOrCreate<UserDetail>();

            //var editProfileViewModel = new EditProfileViewModel()
            //{
            //    Id = user.Id,
            //    DisplayName = user.DisplayName,
            //    Location = data.Profile.Location,
            //    Bio = data.Profile.Bio,
            //    Url = data.Profile.Url
            //};

            var editProfileViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id
            };

            // Build view
            var result = await _editProfileViewProvider.ProvideEditAsync(editProfileViewModel, this);

            // Return view
            return View(result);

        }

        [HttpPost]
        [ActionName(nameof(EditProfile))]
        public async Task<IActionResult> EditProfilePost(EditNotificationsViewModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }


            //// Validate model state within all view providers
            //if (await _editProfileViewProvider.IsModelStateValid(model, this))
            //{
            //    var result = await _editProfileViewProvider.ProvideUpdateAsync(model, this);

            //    // Ensure modelstate is still valid after view providers have executed
            //    if (ModelState.IsValid)
            //    {
            //        _alerter.Success(T["Profile Updated Successfully!"]);
            //        return RedirectToAction(nameof(EditProfile));
            //    }

            //}

            //// if we reach this point some view model validation
            //// failed within a view provider, display model state errors
            //foreach (var modelState in ViewData.ModelState.Values)
            //{
            //    foreach (var error in modelState.Errors)
            //    {
            //        _alerter.Danger(T[error.ErrorMessage]);
            //    }
            //}

            return await EditProfile();

        }

    }
}
