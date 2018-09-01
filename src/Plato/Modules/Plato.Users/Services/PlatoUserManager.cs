using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Users;

namespace Plato.Users.Services
{

    public class PlatoUserManager : IPlatoUserManager<User>
    {
        public Task<IActivityResult<User>> CreateAsync(string userName, string email, string[] roleNames, string password, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> CreateAsync(User model, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> UpdateAsync(User model, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> DeleteAsync(User model, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> ChangePasswordAsync(User user, string currentPassword, string newPassword, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> GetAuthenticatedUserAsync(ClaimsPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> GetForgotPasswordUserAsync(string userIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<IActivityResult<User>> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword, Action<string, string> reportError)
        {
            throw new NotImplementedException();
        }
    }

}
