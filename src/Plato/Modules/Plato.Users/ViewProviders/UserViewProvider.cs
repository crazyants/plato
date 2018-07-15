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
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<User>
    {

        private readonly UserManager<User> _userManager;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly IUrlHelper _urlHelper;

        public UserViewProvider(
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IHostingEnvironment hostEnvironment,
            IUrlHelperFactory urlHelperFactory,
            IUserPhotoStore<UserPhoto> userPhotoStore)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _userPhotoStore = userPhotoStore;
            _actionContextAccesor = actionContextAccesor;
            _urlHelper = urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
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

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IUpdateModel updater)
        {
            return Task.FromResult(Views(
                View<User>("User.List", model => user).Zone("header").Order(3)
            ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(User user, IUpdateModel updater)
        {

            return Task.FromResult(
                Views(
                    View<User>("Admin.Edit.Header", model => user).Zone("header"),
                    View<User>("Admin.Edit.Meta", model => user).Zone("meta"),
                    View<EditUserViewModel>("Admin.Edit.Content", model =>
                    {
                        model.Id = user.Id;
                        model.UserName = user.UserName;
                        model.Email = user.Email;
                        return model;
                    }).Zone("content"),
                    View<EditUserViewModel>("Admin.Edit.Sidebar", model =>
                    {
                        model.Id = user.Id;
                        model.UserName = user.UserName;
                        model.Email = user.Email;
                        return model;
                    }).Zone("sidebar"),
                    View<EditUserViewModel>("Admin.Edit.Footer", model =>
                    {
                        model.Id = user.Id;
                        model.UserName = user.UserName;
                        model.Email = user.Email;
                        return model;
                    }).Zone("footer"),
                    View<EditUserViewModel>("Admin.Edit.Actions", model =>
                    {
                        model.Id = user.Id;
                        model.UserName = user.UserName;
                        model.Email = user.Email;
                        return model;
                    }).Zone("actions")
                ));

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
        

    }
}
