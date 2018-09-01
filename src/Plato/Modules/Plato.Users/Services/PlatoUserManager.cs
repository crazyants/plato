using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Services
{

    public class PlatoUserManager<TUser> : IPlatoUserManager<TUser> where TUser : class
    {
        
        private readonly UserManager<User> _userManager;
        private readonly IOptions<IdentityOptions> _identityOptions;
        private readonly IStringLocalizer<PlatoUserManager<TUser>> T;
        private readonly IBroker _broker;

        public PlatoUserManager(
            UserManager<User> userManager,
            IOptions<IdentityOptions> identityOptions,
            IStringLocalizer<PlatoUserManager<TUser>> stringLocalizer,
            IBroker broker)
        {
            _userManager = userManager;
            _identityOptions = identityOptions;
            _broker = broker;

            T = stringLocalizer;

        }

        #region "Implementation"

        public async Task<IActivityResult<TUser>> CreateAsync(
            string userName,
            string displayName,
            string email,           
            string password,
            string[] roleNames,
            Action<string, string> reportError)
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
                return result.Failed(new ActivityError("Email", T["The email is already used."]));
            }

            // Build model
            var user = new User
            {
                UserName = userName,
                DisplayName = displayName,
                Email = email,
                RoleNames = new List<string>(roleNames ?? new string[] {})
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

        public Task<IActivityResult<TUser>> UpdateAsync(TUser model, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> DeleteAsync(TUser model, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword, Action<string, string> reportError)
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

        public Task<IActivityResult<TUser>> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword, Action<string, string> reportError)
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

    #endregion

}

}
