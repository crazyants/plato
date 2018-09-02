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

namespace Plato.Users.Services
{

    public class PlatoUserManager<TUser> : IPlatoUserManager<TUser> where TUser : class
    {

        private readonly UserManager<User> _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IStringLocalizer<PlatoUserManager<TUser>> T;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBroker _broker;

        public PlatoUserManager(
            UserManager<User> userManager,
            IOptions<IdentityOptions> identityOptions,
            IStringLocalizer<PlatoUserManager<TUser>> stringLocalizer,
            IHttpContextAccessor httpContextAccessor,
            IBroker broker)
        {
            _userManager = userManager;
            _identityOptions = identityOptions;
            _httpContextAccessor = httpContextAccessor;
            _broker = broker;

            T = stringLocalizer;

        }

        #region "Implementation"

        public async Task<IActivityResult<TUser>> CreateAsync(
            string userName,
            string displayName,
            string email,
            string password,
            string[] roleNames = null)
        {

            var result = new ActivityResult<TUser>();

            // Validate

            if (String.IsNullOrEmpty(userName) || String.IsNullOrWhiteSpace(userName))
            {
                return result.Failed(new ActivityError("Username", T["A username is required"]));
            }

            if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email))
            {
                return result.Failed(new ActivityError("Email", T["A email is required"]));
            }

            if (string.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(password))
            {
                return result.Failed(new ActivityError("Password", T["A password is required"]));
            }

            // Check Uniqueness

            // Is this a unique email?
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return result.Failed(new ActivityError("Email", T["The email already exists"]));
            }

            var roles = new List<string>(roleNames ?? new string[] {DefaultRoles.Member});

            // Build model
            var user = new User
            {
                UserName = userName,
                DisplayName = displayName,
                Email = email,
                RoleNames = roles,
                IpV4Address = GetIpV4Address(),
                IpV6Address = GetIpV6Address()
            };

            // Invoke UserCreating subscriptions
            foreach (var handler in await _broker.Pub<User>(this, new MessageOptions()
            {
                Key = "UserCreating"
            }, user))
            {
                user = await handler.Invoke(new Message<User>(user, this));
            }

            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                var errors = new List<ActivityError>();
                foreach (var error in identityResult.Errors)
                {
                    errors.Add(new ActivityError(error.Code, T[error.Description]));
                }

                return result.Failed(errors.ToArray());
            }

            // Add new roles
            foreach (var role in roles)
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            // Invoke UserCreated subscriptions
            foreach (var handler in await _broker.Pub<User>(this, new MessageOptions()
            {
                Key = "UserCreated"
            }, user))
            {
                user = await handler.Invoke(new Message<User>(user, this));
            }

            // Return success
            return result.Success(user);

        }

        public Task<IActivityResult<TUser>> UpdateAsync(TUser model)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> DeleteAsync(TUser model)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> GetForgotPasswordUserAsync(string userIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> ResetPasswordAsync(string userIdentifier, string resetToken,
            string newPassword)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Private Methods"

        //private void ProcessValidationErrors(IEnumerable<IdentityError> errors, User user, Action<string, string> reportError)
        //{
        //    foreach (var error in errors)
        //    {
        //        switch (error.Code)
        //        {
        //             Password
        //            case "PasswordRequiresDigit":
        //                reportError("Password", T["Passwords must have at least one digit ('0'-'9')."]);
        //        break;
        //            case "PasswordRequiresLower":
        //                reportError("Password", T["Passwords must have at least one lowercase ('a'-'z')."]);
        //        break;
        //            case "PasswordRequiresUpper":
        //                reportError("Password", T["Passwords must have at least one uppercase('A'-'Z')."]);
        //        break;
        //            case "PasswordRequiresNonAlphanumeric":
        //                reportError("Password", T["Passwords must have at least one non letter or digit character."]);
        //        break;
        //            case "PasswordTooShort":
        //                reportError("Password", T["Passwords must be at least {0} characters.", _identityOptions.Value.Password.RequiredLength]);
        //        break;
        //            case "PasswordRequiresUniqueChars":
        //                reportError("Password", T["Passwords must contain at least {0} unique characters.", _identityOptions.Value.Password.RequiredUniqueChars]);
        //        break;

        //        CurrentPassword
        //            case "PasswordMismatch":
        //                reportError("CurrentPassword", T["Incorrect password."]);
        //        break;

        //        User name
        //            case "InvalidUserName":
        //                reportError("UserName", T["User name '{0}' is invalid, can only contain letters or digits.", user.UserName]);
        //        break;
        //            case "DuplicateUserName":
        //                reportError("UserName", T["User name '{0}' is already used.", user.UserName]);
        //        break;

        //        Email
        //            case "InvalidEmail":
        //                reportError("Email", T["Email '{0}' is invalid.", user.Email]);
        //        break;
        //        default:
        //                reportError(string.Empty, T["Unexpected error: '{0}'.", error.Code]);
        //        break;
        //    }
        //}
        // }

        string GetIpV4Address(bool tryUseXForwardHeader = true)
        {

            //const string localIpAddress = ":::1";
            const string forwardForHeader = "X-Forwarded-For";
            const string remoteAddrHeader = "REMOTE_ADDR";
            var value = string.Empty;

            // Check X-Forwarded-For
            // Is the request forwarded via a proxy server?
            if (tryUseXForwardHeader)
            {
                value = _httpContextAccessor.GetRequestHeaderValueAs<string>(forwardForHeader)?.Split(',').FirstOrDefault();
            }

            // Get REMOTE_ADDR header
            if (value == string.Empty)
            {
                value = _httpContextAccessor.GetRequestHeaderValueAs<string>(remoteAddrHeader);
            }

            // Nothing yet, check .NET core implementation
            if (value == string.Empty)
            {
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
                    ip = System.Net.Dns.GetHostEntry(ip).AddressList
                        .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }

            }

            return ip == null ? string.Empty : ip.ToString();

        }

        string GetIpV6Address()
        {

            return string.Empty;

        }
        

        #endregion

    }

}
