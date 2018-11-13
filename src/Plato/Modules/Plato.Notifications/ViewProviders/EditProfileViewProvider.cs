using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
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

        public EditProfileViewProvider(UserManager<User> userManager, IPlatoUserStore<User> platoUserStore, INotificationTypeManager<NotificationType> notificationTypeManager)
        {
            _userManager = userManager;
            _platoUserStore = platoUserStore;
            _notificationTypeManager = notificationTypeManager;
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

            var editNotificationsViewModel = new EditNotificationsViewModel()
            {
                Id = user.Id,
                CategorizedNotificationTypes = _notificationTypeManager.GetCategorizedNotificationTypes()
            };
            
            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("sidebar"),
                View<EditNotificationsViewModel>("Home.Edit.Content", model => editNotificationsViewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(EditNotificationsViewModel model,
            IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
    }

}