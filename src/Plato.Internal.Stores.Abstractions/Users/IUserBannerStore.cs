using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IUserBannerStore<T> : IStore<T> where T : class
    {
        Task<T> GetByUserIdAsync(int userId);
    }
}