using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IUserBannerStore<T> : IStore<T> where T : class
    {
        Task<T> GetByUserIdAsync(int userId);
    }
}