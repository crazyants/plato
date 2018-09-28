using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<User>
    {

        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        
        private readonly IUrlHelper _urlHelper;

        private readonly IStringLocalizer T;

        public AdminViewProvider(
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IHostingEnvironment hostEnvironment,
            IUrlHelperFactory urlHelperFactory,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IStringLocalizer<AdminViewProvider> stringLocalizer,
            IPlatoUserManager<User> platoUserManager)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _userPhotoStore = userPhotoStore;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccesor.ActionContext);
            _platoUserManager = platoUserManager;
        
            T = stringLocalizer;
            
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(UserIndexViewModel)] as UserIndexViewModel;
            
            return Task.FromResult(
                Views(
                    View<UserIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header"),
                    View<UserIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools"),
                    View<UserIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content")
                ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(User user, IViewProviderContext updater)
        {

            return Task.FromResult(
                Views(
                    View<User>("Admin.Display.Header", model => user).Zone("header"),
                    View<User>("Admin.Display.Meta", model => user).Zone("meta"),
                    View<User>("Admin.Display.Content", model => user).Zone("content"),
                    View<User>("Admin.Display.Footer", model => user).Zone("footer")
                ));

        }

        public override Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
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
                IsNewUser = user.Id == 0,
                DisplayPasswordFields = user.Id == 0,
                EmailConfirmed = user.EmailConfirmed
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

            var model = new EditUserViewModel
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email
            };

            if (await IsNewUser(user.Id.ToString()))
            {
                if (model.Password != model.PasswordConfirmation)
                {
                    updater.ModelState.AddModelError(nameof(model.PasswordConfirmation), T["Password and Password Confirmation do not match"]);
                }
            }

            var valid = await updater.TryUpdateModelAsync(model);

            return valid;
        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(User user, IViewProviderContext context)
        {

            var model = new EditUserViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(user, context);
            }

            model.UserName = model.UserName?.Trim();
            model.Email = model.Email?.Trim();
            model.DisplayName = model?.DisplayName.Trim();

            if (context.Updater.ModelState.IsValid)
            {

                // Update display name. Username and email address are update via UserManager
                user.DisplayName = model.DisplayName;

                // Update photo
                if (model.AvatarFile != null)
                {
                    await UpdateUserPhoto(user, model.AvatarFile);
                }

                //user.EmailConfirmed = true;

                // Update username and email
                await _userManager.SetUserNameAsync(user, model.UserName);

                // Has the email address changed?
                if (model.Email != null && !model.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Only call SetEmailAsync if the email address changes
                    // SetEmailAsync internally sets EmailConfirmed to "false"
                    await _userManager.SetEmailAsync(user, model.Email);
                }
                
                // As we are updating via the Admin CP set back to "true"
                // await _userEmailStore.SetEmailConfirmedAsync(user, true, CancellationToken.None);

                // Persist changes
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Mark admin created users as confirmed
                    var tokenResult = await _platoUserManager.GetEmailConfirmationUserAsync(user.Email);
                    if (tokenResult.Succeeded)
                    {
                        //var confirmationResult = await _platoUserManager.ConfirmEmailAsync(model.Email,
                        //    tokenResult.Response.ConfirmationToken);
                        //if (!confirmationResult.Succeeded)
                        //{
                        //    foreach (var error in confirmationResult.Errors)
                        //    {
                        //        updater.ModelState.AddModelError(string.Empty, error.Description);
                        //    }
                        //}
                    }
                    else
                    {
                        foreach (var error in tokenResult.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            
            return await BuildEditAsync(user, context);

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

       
        public async Task<bool> IsNewUser(string userId)
        {
            return await _userManager.FindByIdAsync(userId) == null;
        }

        #endregion

    }

}
