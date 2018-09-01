using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Users.Services
{

    public interface IPlatoUserManager<TUser> where TUser : class

    {
        Task<IActivityResult<TUser>> CreateAsync(string userName, string email, string[] roleNames, string password, Action<string, string> reportError);

        Task<IActivityResult<TUser>> CreateAsync(TUser model, Action<string, string> reportError);

        Task<IActivityResult<TUser>> UpdateAsync(TUser model, Action<string, string> reportError);

        Task<IActivityResult<TUser>> DeleteAsync(TUser model, Action<string, string> reportError);

        Task<IActivityResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword, Action<string, string> reportError);

        Task<IActivityResult<TUser>> GetAuthenticatedUserAsync(ClaimsPrincipal principal);

        Task<IActivityResult<TUser>> GetForgotPasswordUserAsync(string userIdentifier);

        Task<IActivityResult<TUser>> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword, Action<string, string> reportError);

    }


}
