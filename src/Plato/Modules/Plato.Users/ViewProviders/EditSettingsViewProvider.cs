using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditSettingsViewProvider : BaseViewProvider<EditSettingsViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        public EditSettingsViewProvider(
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager, IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userPhotoStore = userPhotoStore;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditSettingsViewModel viewModel, IViewProviderContext updater)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }
            
            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("sidebar"),
                View<User>("Home.Edit.Tools", model => user).Zone("tools"),
                View<EditSettingsViewModel>("Home.EditSettings.Content", model => viewModel).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditSettingsViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(viewModel);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditSettingsViewModel viewModel, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, context);
            }

            var model = new EditSettingsViewModel();;

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {
                user.TimeZone = model.TimeZone;
                user.ObserveDst = model.ObserveDst;
                user.Culture = model.Culture;
           
                // Update user
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(viewModel, context);

        }

        #endregion

      
    }

}
