using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        Task<T> SelectByUserNameNormalizedAsync(string userNameNormalized);

        Task<T> SelectByUserNameAsync(string userName);

        Task<T> SelectByEmailAsync(string email);

        Task<T> SelectByEmailNormalizedAsync(string emailNormalized);

        Task<T> SelectByUserNameAndPasswordAsync(string userName, string password);

        Task<T> SelectByEmailAndPasswordAsync(string email, string password);

        Task<T> SelectByResetTokenAsync(string resetToken);
        
        Task<T> SelectByApiKeyAsync(string apiKey);
        
    }

}