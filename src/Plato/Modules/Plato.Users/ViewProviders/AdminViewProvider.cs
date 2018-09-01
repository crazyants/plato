using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IUrlHelper _urlHelper;

        public AdminViewProvider(
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IHostingEnvironment hostEnvironment,
            IUrlHelperFactory urlHelperFactory,
            IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _userPhotoStore = userPhotoStore;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccesor.ActionContext);
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {

            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions
            {
                Page = GetPageIndex(updater)
            };

            var viewModel = new UsersIndexViewModel()
            {
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions
            };

            return Task.FromResult(
                Views(
                    View<UsersIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                    View<UsersIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                    View<UsersIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
                ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IUpdateModel updater)
        {

            return Task.FromResult(
                Views(
                    View<User>("Admin.Display.Header", model => user).Zone("header"),
                    View<User>("Admin.Display.Meta", model => user).Zone("meta"),
                    View<User>("Admin.Display.Content", model => user).Zone("content"),
                    View<User>("Admin.Display.Footer", model => user).Zone("footer")
                ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            var details = user.GetOrCreate<UserDetail>();

            var viewModel = new EditUserViewModel()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                Location = details.Profile.Location,
                Url = details.Profile.Url,
                Bio = details.Profile.Bio,
                LastLoginDate = user.LastLoginDate,
                IsNewUser = user.Id == 0
            };

            return Task.FromResult(
                Views(
                    View<EditUserViewModel>("Admin.Edit.Header", model => viewModel).Zone("header"),
                    View<EditUserViewModel>("Admin.Edit.Meta", model => viewModel).Zone("meta"),
                    View<EditUserViewModel>("Admin.Edit.Content", model => viewModel).Zone("content"),
                    View<EditUserViewModel>("Admin.Edit.Sidebar", model => viewModel).Zone("sidebar"),
                    View<EditUserViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer"),
                    View<EditUserViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions")
                ));

        }


        public override async Task ComposeTypeAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.DisplayName = model.DisplayName;
            }

        }

        public override async Task<bool> ValidateModelAsync(User user, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditUserViewModel
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email
            });
        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IUpdateModel updater)
        {

            var model = new EditUserViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, updater);
            }

            model.UserName = model.UserName?.Trim();
            model.Email = model.Email?.Trim();

            if (updater.ModelState.IsValid)
            {

                if (model.AvatarFile != null)
                {
                    await UpdateUserPhoto(user, model.AvatarFile);
                }

                await _userManager.SetUserNameAsync(user, model.UserName);
                await _userManager.SetEmailAsync(user, model.Email);

                var result = await _userManager.UpdateAsync(user);
                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            
            return await BuildEditAsync(user, updater);

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

        #endregion

    }

}
