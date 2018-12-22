using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Users.Services
{

    public class PlatoUserManager<TUser> : IPlatoUserManager<TUser> where TUser : class, IUser
    {

        public const string ForwardForHeader = "X-Forwarded-For";
        public const string RemoteAddrHeader = "REMOTE_ADDR";

        private readonly UserManager<TUser> _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IStringLocalizer<PlatoUserManager<TUser>> T;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBroker _broker;
        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IPlatoUserStore<TUser> _platoUserStore;
        private readonly IUserColorProvider _userColorProvider;

        public PlatoUserManager(
            IOptions<IdentityOptions> identityOptions,
            IStringLocalizer<PlatoUserManager<TUser>> stringLocalizer,
            UserManager<TUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ISiteSettingsStore siteSettingsStore,
            IPlatoUserStore<TUser> platoUserStore,
            IBroker broker,
            IUserColorProvider userColorProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityOptions = identityOptions;
            _siteSettingsStore = siteSettingsStore;
            _platoUserStore = platoUserStore;
            _userManager = userManager;
            _broker = broker;
            _userColorProvider = userColorProvider;

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


        public async Task<ICommandResult<TUser>> CreateAsync(TUser user)
        {
            return await CreateAsync(user, user.Password);
        }

        public async Task<ICommandResult<TUser>> CreateAsync(TUser user, string password)
        {

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = new CommandResult<TUser>();

            // Validate
            // -------------------------

            if (String.IsNullOrEmpty(user.UserName) || String.IsNullOrWhiteSpace(user.UserName))
            {
                return result.Failed(new CommandError("UserMame", T["A username is required"]));
            }

            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrWhiteSpace(user.Email))
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
            if (await _userManager.FindByEmailAsync(user.Email) != null)
            {
                return result.Failed(new CommandError("Email", T["The email already exists"]));
            }

            // Is this a unique username?
            if (await _userManager.FindByNameAsync(user.UserName.Normalize()) != null)
            {
                return result.Failed(new CommandError("UserMame", T["The username already exists"]));
            }

            // -------------------------

            // Get site settings to populate some user defaults
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                user.TimeZone = settings.TimeZone;
            }
    
            var color = _userColorProvider.GetColor();

            if (color != null)
            {
                user.ForeColor = color.ForeColor;
                user.BackColor = color.BackColor;
            }

            if (String.IsNullOrEmpty(user.IpV4Address))
            {
                user.IpV4Address = GetIpV4Address();
            }

            if (String.IsNullOrEmpty(user.IpV6Address))
            {
                user.IpV6Address = GetIpV6Address();
            }

            // Add new roles
            foreach (var role in user.RoleNames)
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            // Invoke UserCreating subscriptions
            foreach (var handler in _broker.Pub<TUser>(this, "UserCreating"))
            {
                user = await handler.Invoke(new Message<TUser>(user, this));
            }

            // Persist the user
            var identityResult = await _userManager.CreateAsync(user, password);
            if (identityResult.Succeeded)
            {

                // Invoke UserCreated subscriptions
                foreach (var handler in _broker.Pub<TUser>(this, "UserCreated"))
                {
                    user = await handler.Invoke(new Message<TUser>(user, this));
                }

                // Return success
                return result.Success(user);

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

        string GetIpV4Address(bool tryUseXForwardHeader = true)
        {

            var value = string.Empty;

            // Check X-Forwarded-For
            // Is the request forwarded via a proxy?
            if (tryUseXForwardHeader)
            {
                value = _httpContextAccessor.GetRequestHeaderValueAs<string>(ForwardForHeader)?.Split(',').FirstOrDefault();
            }

            // If no "X-Forwarded-For" header, get REMOTE_ADDR header instead
            if (String.IsNullOrEmpty(value))
            {
                value = _httpContextAccessor.GetRequestHeaderValueAs<string>(RemoteAddrHeader);
            }

            // Nothing yet, check .NET core implementation (HttpContext.Connection)
            if (String.IsNullOrEmpty(value))
            {
                // This can sometimes be null in earlier versions of .NET core
                if (_httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress != null)
                {
                    value = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
            }

            // Translate to valid IP 
            IPAddress ip = null;
            if (!String.IsNullOrEmpty(value))
            {
                // Attempt to parse IP address
                IPAddress.TryParse(value, out ip);
                // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                // This usually only happens when the browser is on the same machine as the server.
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ip = Dns.GetHostEntry(ip).AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
            }

            return ip == null ? string.Empty : ip.ToString();

        }

        string GetIpV6Address()
        {
            var ip = GetIpV4Address();
            if (!String.IsNullOrEmpty(ip))
            {
                var addressList = Dns.GetHostEntry(ip).AddressList;
                if (addressList != null)
                {
                    ip = addressList.FirstOrDefault()?.ToString();
                }
            }
            return ip;
        }

        private async Task<TUser> FindByUsernameOrEmailAsync(string userIdentifier)
        {
            userIdentifier = userIdentifier.Normalize();
            return await _userManager.FindByNameAsync(userIdentifier) ?? 
                   await _userManager.FindByEmailAsync(userIdentifier);
        }

        #endregion

    }

}
