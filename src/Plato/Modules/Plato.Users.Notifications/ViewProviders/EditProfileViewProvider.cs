using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Notifications;
using Plato.Internal.Models.Users;
using Plato.Internal.Notifications.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Notifications.Models;
using Plato.Users.Notifications.ViewModels;

namespace Plato.Users.Notifications.ViewProviders
{

    public class EditProfileViewProvider : BaseViewProvider<EditNotificationsViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly INotificationTypeManager _notificationTypeManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditProfileViewProvider(
            UserManager<User> userManager, 
            IPlatoUserStore<User> platoUserStore,
            INotificationTypeManager notificationTypeManager, 
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _platoUserStore = platoUserStore;
            _notificationTypeManager = notificationTypeManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditNotificationsViewModel model,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(EditNotificationsViewModel model,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditNotificationsViewModel viewModel,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }
            
            // Get all notification types to enable by default
            var defaultNotificationTypes = _notificationTypeManager.GetDefaultNotificationTypes();
            var defaultUserNotificationTypes = new List<UserNotificationType>();
            foreach (var notificationType in defaultNotificationTypes)
            {
                defaultUserNotificationTypes.Add(new UserNotificationType(notificationType.Name));
            }

            // Holds our list of enabled notification types
            var enabledNotificationTypes = new List<UserNotificationType>();

            // Get saved notificaiton types
            var userNotificationSettings = user.GetOrCreate<UserNotificationTypes>();

            // We have previously saved settings
            if (userNotificationSettings.NotificationTypes != null)
            {

                // Add all user specified notification types
                enabledNotificationTypes.AddRange(userNotificationSettings.NotificationTypes);

                // Loop through all available notification types to see if the user has saved
                // a value (on or off) for the notification type, if no value is specified
                // ensure the notification type is added to our list of enabled notification types by default
                foreach (var userNotification in defaultUserNotificationTypes)
                {
                    var foundNotification = userNotificationSettings.NotificationTypes.First(n =>
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
            
            var editNotificationsViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id,
                CategorizedNotificationTypes = _notificationTypeManager.GetCategorizedNotificationTypes(),
                EnabledNotificationTypes = enabledNotificationTypes
            };
            
            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("sidebar"),
                View<EditNotificationsViewModel>("Home.Edit.Content", model => editNotificationsViewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditNotificationsViewModel viewModel,
            IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }

            // Build list of selected notification types
            var request = _httpContextAccessor.HttpContext.Request;
            var notificationTypes = new List<UserNotificationType>();
            foreach (string key in request.Form.Keys)
            {
                
                if (key.StartsWith("Checkbox.") && request.Form[key] == "true")
                {
                    var notificationTypeId = key.Substring("Checkbox.".Length);
                    notificationTypes.Add(new UserNotificationType(notificationTypeId));
                }
            }
            
            if (context.Updater.ModelState.IsValid)
            {

                // Update 
                var model = user.GetOrCreate<UserNotificationTypes>();
                model.NotificationTypes = notificationTypes;
                user.AddOrUpdate<UserNotificationTypes>(model);

                // Persist
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return await BuildEditAsync(viewModel, context);


        }
    }

}