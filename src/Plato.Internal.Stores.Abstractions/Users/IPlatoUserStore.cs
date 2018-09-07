using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IPlatoUserStore<T> : IStore<T> where T : class
    {

        Task<T> GetByUserNameNormalizedAsync(string userNameNormalized);

        Task<T> GetByUserNameAsync(string userName);

        Task<T> GetByEmailAsync(string email);

        Task<T> GetByResetToken(string resetToken);

        Task<T> GetByApiKeyAsync(string apiKey);

    }
}