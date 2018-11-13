using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Notifications.Models;
using Plato.Notifications.Services;
using Plato.Notifications.ViewModels;

namespace Plato.Notifications.ViewProviders
{
    public class EditProfileViewProvider : BaseViewProvider<EditNotificationsViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly INotificationTypeManager<NotificationType> _notificationTypeManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditProfileViewProvider(UserManager<User> userManager, IPlatoUserStore<User> platoUserStore, INotificationTypeManager<NotificationType> notificationTypeManager, IHttpContextAccessor httpContextAccessor)
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

            var userNotificationSettings = user.GetOrCreate<UserNotificationSettings>();

            var editNotificationsViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id,
                CategorizedNotificationTypes = _notificationTypeManager.GetCategorizedNotificationTypes(),
                EnabledNotificationTypes = userNotificationSettings.NotificationTypes
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
            var notificationTypes = new List<NotificationType>();
            foreach (string key in request.Form.Keys)
            {
                
                if (key.StartsWith("Checkbox.") && request.Form[key] == "true")
                {
                    var notificationTypeName = key.Substring("Checkbox.".Length);
                    notificationTypes.Add(new NotificationType(notificationTypeName));
                }
            }

         
            if (context.Updater.ModelState.IsValid)
            {

                // Update 
                var model = user.GetOrCreate<UserNotificationSettings>();
                model.NotificationTypes = notificationTypes;
                user.AddOrUpdate<UserNotificationSettings>(model);

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