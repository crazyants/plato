using System.Security.Claims;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Users.Services
{

    public interface IPlatoUserManager<TUser> : ICommandManager<TUser> where TUser : class
    {

        Task<ICommandResult<TUser>> CreateAsync(
            string userName,
            string displayName,
            string email,
            string password,
            string[] roleNames = null);

        Task<ICommandResult<TUser>> CreateAsync(
            string userName,
            string email,
            string password,
            string[] roleNames = null);

        Task<ICommandResult<TUser>> CreateAsync(
            TUser user,
            string password);

        Task<ICommandResult<TUser>> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

        Task<ICommandResult<TUser>> GetAuthenticatedUserAsync(ClaimsPrincipal principal);

        Task<ICommandResult<TUser>> GetForgotPasswordUserAsync(string userIdentifier);

        Task<ICommandResult<TUser>> GetEmailConfirmationUserAsync(string userIdentifier);
        
        Task<ICommandResult<TUser>> ResetPasswordAsync(string userIdentifier, string resetToken, string newPassword);

        Task<ICommandResult<TUser>> ConfirmEmailAsync(string userIdentifier, string confirmationToken);
        
    }


}
