using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IUserPhotoStore<T> : IStore<T> where T : class
    {
        Task<T> GetByUserIdAsync(int userId);
    }
}