using System.Threading.Tasks;
using Plato.Abstractions.Query;

namespace Plato.Internal.Repositories.Users
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        Task<T> SelectByUserNameNormalizedAsync(string userNameNormalized);

        Task<T> SelectByUserNameAsync(string userName);

        Task<T> SelectByEmailAsync(string email);

        Task<T> SelectByUserNameAndPasswordAsync(string userName, string password);

        Task<T> SelectByEmailAndPasswordAsync(string email, string password);

        Task<T> SelectByApiKeyAsync(string apiKey);
        
    }

}