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

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile user, IUpdateModel updater)
        {

            var viewOptions = new ViewOptions
            {
                Search = GetKeywords(updater)
            };

            var pagerOptions = new PagerOptions
            {
                Page = GetPageIndex(updater)
            };

            var viewModel = new UsersIndexViewModel()
            {
                ViewOpts = viewOptions,
                PagerOpts = pagerOptions
            };

            return Task.FromResult(
                Views(
                    View<UsersIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                    View<UsersIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                    View<UsersIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
                ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile, IUpdateModel updater)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }

            return Views(
                View<User>("Home.Display.Header", model => user).Zone("header"),
                View<User>("Home.Display.Tools", model => user).Zone("tools"),
                View<User>("Home.Display.Content", model => user).Zone("content"),
                View<User>("Home.Display.Sidebar", model => user).Zone("sidebar")
            );
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile userProfile, IUpdateModel updater)
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

        public override async Task<IViewProviderResult> BuildUpdateAsync(UserProfile userProfile, IUpdateModel updater)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }
            
            var model = new EditUserViewModel();

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
        
        int GetPageIndex(IUpdateModel updater)
        {

            var page = 1;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }

        string GetKeywords(IUpdateModel updater)
        {

            var keywords = string.Empty;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("search", out object value);
            if (found)
            {
                keywords = value.ToString();
            }

            return keywords;

        }

        #endregion

    }
}
