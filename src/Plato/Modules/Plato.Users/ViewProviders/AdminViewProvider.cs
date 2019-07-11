using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.Services;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<User>
    {

        private static string _pathToImages;
        private static string _urlToImages;

        private const string BySignatureHtmlName = "Signature";

        private readonly IPlatoUserStore<User> _userStore;
        private readonly IPlatoUserManager<User> _platoUserManager;
        private readonly UserManager<User> _userManager;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly IUserPhotoStore<UserPhoto> _userPhotoStore;
        private readonly ISitesFolder _sitesFolder;

        private readonly IUrlHelper _urlHelper;

        private readonly IStringLocalizer T;

        public AdminViewProvider(
            IShellSettings shellSettings,
            UserManager<User> userManager,
            IActionContextAccessor actionContextAccesor,
            IUrlHelperFactory urlHelperFactory,
            IUserPhotoStore<UserPhoto> userPhotoStore,
            IStringLocalizer<AdminViewProvider> stringLocalizer,
            IPlatoUserManager<User> platoUserManager,
            IHostingEnvironment hostEnvironment,
            IFileStore fileStore,
            ISitesFolder sitesFolder,
            IPlatoUserStore<User> userStore)
        {
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
            _sitesFolder = sitesFolder;
            _userStore = userStore;
            _userPhotoStore = userPhotoStore;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccesor.ActionContext);
            _platoUserManager = platoUserManager;
        
            T = stringLocalizer;

            // paths
            _pathToImages = fileStore.Combine(hostEnvironment.ContentRootPath, shellSettings.Location, "images");
            _urlToImages = $"/sites/{shellSettings.Location.ToLower()}/images/";
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(User user, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(UserIndexViewModel)] as UserIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(UserIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

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

        public override async Task<IViewProviderResult> BuildEditAsync(User user, IViewProviderContext updater)
        {

            var details = user.GetOrCreate<UserDetail>();

            User isVerifiedBy = null;
            User isSpamBy = null;
            User isBannedBy = null;

            if (user.IsVerified && user.IsVerifiedUpdatedUserId > 0)
            {
                isVerifiedBy = await _userStore.GetByIdAsync(user.IsVerifiedUpdatedUserId);
            }

            if (user.IsSpam && user.IsSpamUpdatedUserId > 0)
            {
                isSpamBy = await _userStore.GetByIdAsync(user.IsSpamUpdatedUserId);
            }

            if (user.IsBanned && user.IsBannedUpdatedUserId > 0)
            {
                isBannedBy = await _userStore.GetByIdAsync(user.IsBannedUpdatedUserId);
            }
            
            var viewModel = new EditUserViewModel()
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                Avatar = user.Avatar,
                Css = user.Css,
                Location = user.Location,
                Url = user.Url,
                Biography = user.Biography,
                Signature = user.Signature,
                SignatureHtml = user.SignatureHtml,
                SignatureHtmlName = BySignatureHtmlName,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                IsNewUser = user.Id == 0,
                DisplayPasswordFields = user.Id == 0,
                EmailConfirmed = user.EmailConfirmed,
                IsSpam = user.IsSpam,
                IsSpamUpdatedUser = isSpamBy != null ? new SimpleUser(isSpamBy) : null,
                IsSpamUpdatedDate = user.IsSpamUpdatedDate,
                IsVerified = user.IsVerified,
                IsVerifiedUpdatedUser = isVerifiedBy != null ? new SimpleUser(isVerifiedBy) : null,
                IsVerifiedUpdatedDate = user.IsVerifiedUpdatedDate,
                IsBanned = user.IsBanned,
                IsBannedUpdatedUser = isBannedBy != null ? new SimpleUser(isBannedBy) : null,
                IsBannedUpdatedDate = user.IsBannedUpdatedDate
            };

            return Views(
                    View<EditUserViewModel>("Admin.Edit.Header", model => viewModel).Zone("header"),
                    View<EditUserViewModel>("Admin.Edit.Meta", model => viewModel).Zone("meta"),
                    View<EditUserViewModel>("Admin.Edit.Content", model => viewModel).Zone("content"),
                    View<EditUserViewModel>("Admin.Edit.Sidebar", model => viewModel).Zone("sidebar"),
                    View<EditUserViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer"),
                    View<EditUserViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions")
                );

        }
        
        public override async Task ComposeModelAsync(User user, IUpdateModel updater)
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
                user.Biography = model.Biography;
                user.Location = model.Location;
                user.Signature = model.Signature;
                user.Url = model.Url;

                // Update photo
                if (model.AvatarFile != null)
                {
                    user.PhotoUrl = await UpdateUserPhoto(user, model.AvatarFile);
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
                
                // Persist changes
                var result = await _platoUserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    
                    // TODO Move to CreatePost
                    // Mark admin created users as confirmed
                    //var tokenResult = await _platoUserManager.GetEmailConfirmationUserAsync(result.Response.Email);
                    //if (tokenResult.Succeeded)
                    //{
                    //    var confirmationResult = await _platoUserManager.ConfirmEmailAsync(model.Email,
                    //        tokenResult.Response.ConfirmationToken);
                    //    if (!confirmationResult.Succeeded)
                    //    {
                    //        foreach (var error in confirmationResult.Errors)
                    //        {
                    //            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var error in tokenResult.Errors)
                    //    {
                    //        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    //    }
                    //}

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

        async Task<string> UpdateUserPhoto(User user, IFormFile file)
        {

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var stream = file.OpenReadStream();
            byte[] bytes = null;
            if (stream != null)
            {
                bytes = stream.StreamToByteArray();
            }

            // Ensure we have a valid byte array
            if (bytes == null)
            {
                return string.Empty;
            }

            // Get any existing photo
            var existingPhoto = await _userPhotoStore.GetByUserIdAsync(user.Id);

            // Upload the new file
            var fileName = await _sitesFolder.SaveUniqueFileAsync(stream, file.FileName, _pathToImages);

            // Ensure the new file was created
            if (!string.IsNullOrEmpty(fileName))
            {
                // Delete any existing file
                if (existingPhoto != null)
                {
                    _sitesFolder.DeleteFile(existingPhoto.Name, _pathToImages);
                }
            }

            // Insert or update photo entry
            var id = existingPhoto?.Id ?? 0;
            var userPhoto = new UserPhoto
            {
                Id = id,
                UserId = user.Id,
                Name = fileName,
                ContentType = file.ContentType,
                ContentLength = file.Length,
                ContentBlob = bytes,
                CreatedUserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            var newOrUpdatedPhoto = id > 0
                ? await _userPhotoStore.UpdateAsync(userPhoto)
                : await _userPhotoStore.CreateAsync(userPhoto);
            if (newOrUpdatedPhoto != null)
            {
                return _urlToImages + newOrUpdatedPhoto.Name;
            }

            return string.Empty;

        }


        public async Task<bool> IsNewUser(string userId)
        {
            return await _userManager.FindByIdAsync(userId) == null;
        }

        #endregion

    }

}
