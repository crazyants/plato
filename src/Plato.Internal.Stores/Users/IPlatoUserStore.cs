using Plato.Abstractions.Stores;
using Plato.Models.Users;
using System.Threading.Tasks;

namespace Plato.Internal.Stores.Users
{
    public interface IPlatoUserStore<T> : IStore<T> where T : class
    {

        Task<T> GetByUserNameNormalizedAsync(string userNameNormalized);

        Task<T> GetByUserNameAsync(string userName);

        Task<T> GetByEmailAsync(string email);

        Task<T> GetByApiKeyAsync(string apiKey);

    }
}