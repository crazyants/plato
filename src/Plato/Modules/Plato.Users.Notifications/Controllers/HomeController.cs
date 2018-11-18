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
using Plato.Users.Notifications.ViewModels;

namespace Plato.Users.Notifications.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {
        private readonly IViewProviderManager<EditNotificationsViewModel> _editProfileViewProvider;
        private readonly IContextFacade _contextFacade;
        private readonly UserManager<User> _userManager;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }


        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            IViewProviderManager<EditNotificationsViewModel> editProfileViewProvider,
            UserManager<User> userManager,
            IAlerter alerter)
        {
            _contextFacade = contextFacade;
            _editProfileViewProvider = editProfileViewProvider;
            _userManager = userManager;
            _alerter = alerter;

            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> EditProfile()
        {

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user == null)
            {
                return NotFound();
            }
            
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

            var editProfileViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id
            };

            // Build view
            await _editProfileViewProvider.ProvideUpdateAsync(editProfileViewModel, this);
            
            // Ensure modelstate is still valid after view providers have executed
            if (ModelState.IsValid)
            {
                _alerter.Success(T["Notifications Updated Successfully!"]);
                return RedirectToAction(nameof(EditProfile));
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

    }
}
