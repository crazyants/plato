using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Notifications.Abstractions;
using Plato.Notifications.Models;
using Plato.Users.Notifications.ViewModels;

namespace Plato.Users.Notifications.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IViewProviderManager<EditNotificationsViewModel> _editProfileViewProvider;
        private readonly INotificationTypeManager _notificationTypeManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly UserManager<User> _userManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }
        
        public HomeController(
            IHtmlLocalizer htmlLocalizer,
            IStringLocalizer stringLocalizer,
            IViewProviderManager<EditNotificationsViewModel> editProfileViewProvider,
            INotificationTypeManager notificationTypeManager,
            IAlerter alerter, IBreadCrumbManager breadCrumbManager,
            UserManager<User> userManager, 
            IContextFacade contextFacade)
        {
            
            _notificationTypeManager = notificationTypeManager;
            _editProfileViewProvider = editProfileViewProvider;
            _breadCrumbManager = breadCrumbManager;
            _contextFacade = contextFacade;
            _userManager = userManager;
            _alerter = alerter;
        
            T = htmlLocalizer;
            S = stringLocalizer;

        }

        public async Task<IActionResult> EditProfile()
        {
            
            // We need to be authenticated to access notification settings
            var user = await _contextFacade.GetAuthenticatedUserAsync();
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
                ).Add(S["Your Account"]);
            });

            // Get saved notification types
            var userNotificationSettings = user.GetOrCreate<UserNotificationTypes>();
            
            // Get all notification types to enable by default
            var defaultNotificationTypes = _notificationTypeManager.GetDefaultNotificationTypes(user.RoleNames);
            var defaultUserNotificationTypes = new List<UserNotificationType>();
            foreach (var notificationType in defaultNotificationTypes)
            {
                defaultUserNotificationTypes.Add(new UserNotificationType(notificationType.Name));
            }

            // Holds our list of enabled notification types
            var enabledNotificationTypes = new List<UserNotificationType>();
            
            // We have previously saved settings
            if (userNotificationSettings.NotificationTypes != null)
            {
                // Add all user specified notification types
                enabledNotificationTypes.AddRange(userNotificationSettings.NotificationTypes);

                // Loop through all default notification types to see if the user has saved
                // a value (on or off) for that notification type, if no value have been previously saved
                // ensure the default notification type is added to our list of enabled notification types
                foreach (var userNotification in defaultUserNotificationTypes)
                {
                    var foundNotification = enabledNotificationTypes.FirstOrDefault(n =>
                        n.Name.Equals(userNotification.Name, StringComparison.OrdinalIgnoreCase));
                    if (foundNotification == null)
                    {
                        enabledNotificationTypes.Add(userNotification);
                    }
                }
            }
            else
            {
                // If we don't have any notification types ensure we enable all by default
                enabledNotificationTypes.AddRange(defaultUserNotificationTypes);
            }
     
            var editProfileViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id
            };
            
            // Return view
            return View((LayoutViewModel) await _editProfileViewProvider.ProvideEditAsync(editProfileViewModel, this));

        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(EditProfile))]
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
            
            // Ensure model state is still valid after view providers have executed
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
