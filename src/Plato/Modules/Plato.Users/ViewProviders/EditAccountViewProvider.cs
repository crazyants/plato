using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{

    public class EditAccountViewProvider : BaseViewProvider<EditAccountViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        public EditAccountViewProvider(
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager, IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userPhotoStore = userPhotoStore;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditAccountViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditAccountViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditAccountViewModel viewModel, IUpdateModel updater)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }
            
            return Views(
                View<User>("Profile.Edit.Header", model => user).Zone("header"),
                View<User>("Profile.Edit.Sidebar", model => user).Zone("sidebar"),
                View<EditAccountViewModel>("Profile.EditAccount.Content", model => viewModel).Zone("content"),
                View<User>("Profile.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditAccountViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditUserViewModel
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email
            });
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditAccountViewModel userProfile, IUpdateModel updater)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }

            var model = new EditAccountViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(userProfile, updater);
            }

            if (updater.ModelState.IsValid)
            {

                // Update user
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(userProfile, updater);

        }

        #endregion

      
    }

}
