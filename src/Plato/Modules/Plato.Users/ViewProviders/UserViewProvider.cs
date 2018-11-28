using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly UserManager<User> _userManager;
        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            UserManager<User> userManager, IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userPhotoStore = userPhotoStore;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile user, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(UserIndexViewModel)] as UserIndexViewModel;
            
            return Task.FromResult(
                Views(
                    View<UserIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                    View<UserIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                    View<UserIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
                ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }

            return Views(
                View<User>("Home.Display.Header", model => user).Zone("header"),
                //View<User>("Home.Display.Tools", model => user).Zone("tools"),
                View<User>("Home.Display.Content", model => user).Zone("content"),
                View<User>("Home.Display.Sidebar", model => user).Zone("sidebar")
            );
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile userProfile, IViewProviderContext context)
        {

            return Task.FromResult(default(IViewProviderResult));

            //var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            //if (user == null)
            //{
            //    return await BuildIndexAsync(userProfile, updater);
            //}

            //var details = user.GetOrCreate<UserDetail>();

            //var viewModel = new EditUserViewModel()
            //{
            //    Id = user.Id,
            //    DisplayName = user.DisplayName,
            //    UserName = user.UserName,
            //    Email = user.Email,
            //    Location = details.Profile.Location,
            //    Bio = details.Profile.Bio
            //};

            //return Views(
            //    View<EditUserViewModel>("Profile.Edit.Header", model => viewModel).Zone("header"),
            //    View<EditUserViewModel>("Profile.Edit.Tools", model => viewModel).Zone("tools"),
            //    View<EditUserViewModel>("Profile.Edit.Content", model => viewModel).Zone("content"),
            //    View<EditUserViewModel>("Profile.Edit.Footer", model => viewModel).Zone("footer"),
            //    View<EditUserViewModel>("Profile.Edit.Sidebar", model => viewModel).Zone("sidebar")
            //);

        }
        
        public override async Task<bool> ValidateModelAsync(UserProfile user, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditUserViewModel
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email
            });
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(UserProfile userProfile, IViewProviderContext context)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, context);
            }
            
            var model = new EditUserViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(userProfile, context);
            }

            if (context.Updater.ModelState.IsValid)
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

                // Update username and email

                await _userManager.SetUserNameAsync(user, model.UserName);
                
                // Has the email address changed?
                if (model.Email != null && !model.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Only call SetEmailAsync if the email address changes
                    // SetEmailAsync internally sets EmailConfirmed to "false"
                    await _userManager.SetEmailAsync(user, model.Email);
                }
                
                // Update user
                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            
            return await BuildEditAsync(userProfile, context);

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
