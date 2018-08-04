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

    public class EditProfileViewProvider : BaseViewProvider<EditProfileViewModel>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        public EditProfileViewProvider(
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager, IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userPhotoStore = userPhotoStore;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(EditProfileViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(EditProfileViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(EditProfileViewModel viewModel, IUpdateModel updater)
        {

            var user = await _platoUserStore.GetByIdAsync(viewModel.Id);
            if (user == null)
            {
                return await BuildIndexAsync(viewModel, updater);
            }
            
            return Views(
                View<User>("Profile.Edit.Header", model => user).Zone("header"),
                View<User>("Profile.Edit.Sidebar", model => user).Zone("sidebar"),
                View<EditProfileViewModel>("Profile.EditProfile.Content", model => viewModel).Zone("content"),
                View<User>("Profile.Edit.Footer", model => user).Zone("footer")
            );

        }

        public override async Task<bool> ValidateModelAsync(EditProfileViewModel viewModel, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditUserViewModel
            {
                DisplayName = viewModel.DisplayName,
                Location = viewModel.Location,
                Bio = viewModel.Bio
            });
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(EditProfileViewModel userProfile, IUpdateModel updater)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }

            var model = new EditProfileViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(userProfile, updater);
            }

            if (updater.ModelState.IsValid)
            {

                // Update user data

                var details = user.GetOrCreate<UserDetail>();
                details.Profile.Location = model.Location;
                details.Profile.Bio = model.Bio;
                user.AddOrUpdate<UserDetail>(details);

                // Update user avatar

                if (model.AvatarFile != null)
                {
                    await UpdateUserPhoto(user, model.AvatarFile);
                }

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

        #region "Private Methods"

        async Task UpdateUserPhoto(User user, IFormFile file)
        {

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            byte[] bytes = null;
            var stream = file.OpenReadStream();
            if (stream != null)
            {
                bytes = stream.StreamToByteArray();
            }
            if (bytes == null)
            {
                return;
            }

            var id = 0;
            var existingPhoto = await _userPhotoStore.GetByUserIdAsync(user.Id);
            if (existingPhoto != null)
            {
                id = existingPhoto.Id;
            }

            var userPhoto = new UserPhoto
            {
                Id = id,
                UserId = user.Id,
                Name = file.FileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            if (id > 0)
                userPhoto = await _userPhotoStore.UpdateAsync(userPhoto);
            else
                userPhoto = await _userPhotoStore.CreateAsync(userPhoto);

        }
        
        #endregion

    }

}
