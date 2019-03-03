using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Services
{

    public class PlatoUserManager<TUser> : IPlatoUserManager<TUser> where TUser : class, IUser
    {
        
        private readonly UserManager<TUser> _userManager;
        private readonly IBroker _broker;
        private readonly IClientIpAddress _clientIpAddress;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IPlatoUserStore<TUser> _platoUserStore;
        private readonly IUserColorProvider _userColorProvider;

        private readonly IStringLocalizer<PlatoUserManager<TUser>> T;

        public PlatoUserManager(
            IOptions<IdentityOptions> identityOptions,
            IStringLocalizer<PlatoUserManager<TUser>> stringLocalizer,
            UserManager<TUser> userManager,
            ISiteSettingsStore siteSettingsStore,
            IPlatoUserStore<TUser> platoUserStore,
            IUserColorProvider userColorProvider, 
            IClientIpAddress clientIpAddress,
            IBroker broker)
        {
            _siteSettingsStore = siteSettingsStore;
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _userColorProvider = userColorProvider;
            _clientIpAddress = clientIpAddress;
            _broker = broker;

            T = stringLocalizer;

        }

        #region "Implementation"

        public async Task<ICommandResult<TUser>> CreateAsync(
            string userName,
            string displayName,
            string email,
            string password,
            string[] roleNames = null)
        {

            // Build user
            var user = ActivateInstanceOf<TUser>.Instance();
            user.UserName = userName;
            user.DisplayName = displayName;
            user.Email = email;
            user.RoleNames = new List<string>(roleNames ?? new string[] { DefaultRoles.Member });
    
            return await CreateAsync(user, password);

        }

        public async Task<ICommandResult<TUser>> CreateAsync(
            string userName,
            string email,
            string password,
            string[] roleNames = null)
        {
            return await CreateAsync(userName, string.Empty, email, password, roleNames);
        }
        
        public async Task<ICommandResult<TUser>> CreateAsync(TUser model)
        {
            return await CreateAsync(model, model.Password);
        }

        public async Task<ICommandResult<TUser>> CreateAsync(TUser model, string password)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new CommandResult<TUser>();

            // Validate
            // -------------------------

            if (String.IsNullOrEmpty(model.UserName) || String.IsNullOrWhiteSpace(model.UserName))
            {
                return result.Failed(new CommandError("UserMame", T["A username is required"]));
            }

            if (String.IsNullOrEmpty(model.Email) || String.IsNullOrWhiteSpace(model.Email))
            {
                return result.Failed(new CommandError("Email", T["A email is required"]));
            }

            if (string.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(password))
            {
                return result.Failed(new CommandError("Password", T["A password is required"]));
            }

            // Check Uniqueness
            // -------------------------

            // Is this a unique email?
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return result.Failed(new CommandError("Email", T["The email already exists"]));
            }

            // Is this a unique username?
            if (await _userManager.FindByNameAsync(model.UserName.Normalize()) != null)
            {
                return result.Failed(new CommandError("UserMame", T["The username already exists"]));
            }

            // -------------------------

            // Get site settings to populate some user defaults
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                model.TimeZone = settings.TimeZone;
            }

            model.PhotoColor = _userColorProvider.GetColor();
            model.SignatureHtml = await ParseSignatureHtml(model.Signature);

            if (String.IsNullOrEmpty(model.IpV4Address))
            {
                model.IpV4Address = _clientIpAddress.GetIpV4Address();
            }

            if (String.IsNullOrEmpty(model.IpV6Address))
            {
                model.IpV6Address = _clientIpAddress.GetIpV6Address();
            }

            // Add new roles
            foreach (var role in model.RoleNames)
            {
                if (!await _userManager.IsInRoleAsync(model, role))
                {
                    await _userManager.AddToRoleAsync(model, role);
                }
            }

            // Invoke UserCreating subscriptions
            foreach (var handler in _broker.Pub<TUser>(this, "UserCreating"))
            {
                model = await handler.Invoke(new Message<TUser>(model, this));
            }

            // Persist the user
            var identityResult = await _userManager.CreateAsync(model, password);
            if (identityResult.Succeeded)
            {

                // Invoke UserCreated subscriptions
                foreach (var handler in _broker.Pub<TUser>(this, "UserCreated"))
                {
                    model = await handler.Invoke(new Message<TUser>(model, this));
                }

                // Return success
                return result.Success(model);

            }

            // User could not be created, accumulate errors
            var errors = new List<CommandError>();
            foreach (var error in identityResult.Errors)
            {
                errors.Add(new CommandError(error.Code, T[error.Description]));
            }

            return result.Failed(errors.ToArray());

        }


        public async Task<ICommandResult<TUser>> UpdateAsync(TUser model)
        {

            var result = new CommandResult<TUser>();
            
            // Validate
            // -------------------------
            
            if (model.Id <= 0)
            {
                return result.Failed(new CommandError("Id", T["You must specify a user id to update"]));
            }
            
            if (String.IsNullOrEmpty(model.UserName) || String.IsNullOrWhiteSpace(model.UserName))
            {
                return result.Failed(new CommandError("UserMame", T["A username is required"]));
            }

            if (String.IsNullOrEmpty(model.Email) || String.IsNullOrWhiteSpace(model.Email))
            {
                return result.Failed(new CommandError("Email", T["A email is required"]));
            }

            // Check Uniqueness
            // -------------------------

            // Is this a unique email?
            var userByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userByEmail != null)
            {
                // found another account with same email
                if (userByEmail.Id != model.Id)
                {
                    return result.Failed(new CommandError("Email", T["The email already exists"]));
                }
            }

            // Is this a unique username?
            var userByUserName = await _userManager.FindByNameAsync(model.UserName.Normalize());
            if (userByUserName != null)
            {
                // found another account with same username
                if (userByUserName.Id != model.Id)
                {
                    return result.Failed(new CommandError("UserMame", T["The username already exists"]));
                }
            }
            
            model.ModifiedDate = DateTimeOffset.Now;
            model.SignatureHtml = await ParseSignatureHtml(model.Signature);

            // Invoke UserUpdating subscriptions
            foreach (var handler in _broker.Pub<TUser>(this, "UserUpdating"))
            {
                model = await handler.Invoke(new Message<TUser>(model, this));
            }

            // -------------------------

            var identityResult = await _userManager.UpdateAsync(model);
            if (identityResult.Succeeded)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                // Invoke UserUpdatied subscriptions
                foreach (var handler in _broker.Pub<TUser>(this, "UserUpdatied"))
                {
                    model = await handler.Invoke(new Message<TUser>(model, this));
                }
                
                return result.Success(user);

            }

            var errors = new List<CommandError>();
            foreach (var error in identityResult.Errors)
            {
                errors.Add(new CommandError(error.Code, T[error.Description]));
            }
            return result.Failed(errors.ToArray());

        }

        public async Task<ICommandResult<TUser>> DeleteAsync(TUser model)
        {

            var result = new CommandResult<TUser>();

            // Invoke UserDeleting subscriptions
            foreach (var handler in _broker.Pub<TUser>(this, "UserDeleting"))
            {
                model = await handler.Invoke(new Message<TUser>(model, this));
            }
            
            var identityResult = await _userManager.DeleteAsync(model);
            if (identityResult.Succeeded)
            {

                // Invoke UserDeleted subscriptions
                foreach (var handler in _broker.Pub<TUser>(this, "UserDeleted"))
                {
                    model = await handler.Invoke(new Message<TUser>(model, this));
                }

                return result.Success();

            }
            
            var errors = new List<CommandError>();
            foreach (var error in identityResult.Errors)
            {
                errors.Add(new CommandError(error.Code, T[error.Description]));
            }
            return result.Failed(errors.ToArray());

        }

        public async Task<ICommandResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {

            var result = new CommandResult<TUser>();
            
            var identityResult = await _userManager.ChangePasswordAsync(
                user, 
                currentPassword,
                newPassword);

            if (!identityResult.Succeeded)
            {
                var errors = new List<CommandError>();
                foreach (var error in identityResult.Errors)
                {
                    errors.Add(new CommandError(error.Code, T[error.Description]));
                }

                return result.Failed(errors.ToArray());
            }

            return result.Success();

        }

        public async Task<ICommandResult<TUser>> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
        {
            var result = new CommandResult<TUser>();
            var identity = principal?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                var user = await _platoUserStore.GetByUserNameAsync(identity.Name);
                return result.Success(user);
            }
            return result.Failed();
        }

        public async Task<ICommandResult<TUser>> GetForgotPasswordUserAsync(string userIdentifier)
        {

            var result = new CommandResult<TUser>();

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return result.Failed();
            }

            var user = await FindByUsernameOrEmailAsync(userIdentifier);
            if (user == null)
            {
                return result.Failed();
            }

            // Generate password reset token
            user.ResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Persist the password reset token
            var updatedUser = await _platoUserStore.UpdateAsync(user);
            if (updatedUser != null)
            {
                return result.Success(updatedUser);
            }
            
            return result.Failed();

        }

        public async Task<ICommandResult<TUser>> GetEmailConfirmationUserAsync(string userIdentifier)
        {
            var result = new CommandResult<TUser>();

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return result.Failed();
            }

            var user = await FindByUsernameOrEmailAsync(userIdentifier);
            if (user == null)
            {
                return result.Failed();
            }

            // Generate email confirmation reset token
            user.ConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Persist the email confirmation token
            var updatedUser = await _platoUserStore.UpdateAsync(user);
            if (updatedUser != null)
            {
                return result.Success(updatedUser);
            }

            return result.Failed();

        }

        public async Task<ICommandResult<TUser>> ResetPasswordAsync(
            string userIdentifier,
            string resetToken,
            string newPassword)
        {

            var result = new CommandResult<TUser>();
            
            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return result.Failed(new CommandError("UserName", T["A user name or email is required"]));
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return result.Failed(new CommandError("Password", T["A password is required"]));
            }

            if (string.IsNullOrWhiteSpace(resetToken))
            {
                return result.Failed(new CommandError("Token", T["A token is required."]));
            }
            
            var user = await FindByUsernameOrEmailAsync(userIdentifier);
            if (user == null)
            {
                return result.Failed();
            }

            var identityResult = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!identityResult.Succeeded)
            {
                var errors = new List<CommandError>();
                foreach (var error in identityResult.Errors)
                {
                    errors.Add(new CommandError(error.Code, T[error.Description]));
                }

                return result.Failed(errors.ToArray());
            }

            return result.Success();

        }

        public async Task<ICommandResult<TUser>> ConfirmEmailAsync(
            string userIdentifier,
            string confirmationToken)
        {
            var result = new CommandResult<TUser>();

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return result.Failed(new CommandError("UserName", T["A user name or email is required"]));
            }

            if (string.IsNullOrWhiteSpace(confirmationToken))
            {
                return result.Failed(new CommandError("Token", T["A confirmation token is required."]));
            }

            var user = await FindByUsernameOrEmailAsync(userIdentifier);
            if (user == null)
            {
                return result.Failed();
            }

            var identityResult = await _userManager.ConfirmEmailAsync(user, confirmationToken);
            if (!identityResult.Succeeded)
            {
                var errors = new List<CommandError>();
                foreach (var error in identityResult.Errors)
                {
                    errors.Add(new CommandError(error.Code, T[error.Description]));
                }

                return result.Failed(errors.ToArray());

            }

            return result.Success(user);
                   
        }

        #endregion

        #region "Private Methods"

        private async Task<TUser> FindByUsernameOrEmailAsync(string userIdentifier)
        {
            userIdentifier = userIdentifier.Normalize();
            return await _userManager.FindByNameAsync(userIdentifier) ?? 
                   await _userManager.FindByEmailAsync(userIdentifier);
        }

        async Task<string> ParseSignatureHtml(string signature)
        {

            foreach (var handler in _broker.Pub<string>(this, "ParseSignatureHtml"))
            {
                signature = await handler.Invoke(new Message<string>(signature, this));
            }

            return signature;

        }

        #endregion

    }

}
