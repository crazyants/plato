using Plato.Abstractions.Stores;
using Plato.Models.Users;
using System.Threading.Tasks;

namespace Plato.Stores.Users
{
    public interface IPlatoUserStore : IStore<User>
    {

        Task<User> GetByUserNameNormalizedAsync(string userNameNormalized);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByApiKeyAsync(string apiKey);

    }
}