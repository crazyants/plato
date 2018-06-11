using System.Threading.Tasks;
using Plato.Abstractions.Stores;

namespace Plato.Internal.Stores.Users
{
    public interface IUserBannerStore<T> : IStore<T> where T : class
    {
        Task<T> GetByUserIdAsync(int userId);
    }
}