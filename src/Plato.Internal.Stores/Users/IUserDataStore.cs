using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IUserDataStore
    {

        Task<T> GetAsync<T>(int userId, string key);

        Task<T> UpdateAsync<T>(int userId, string key, ISerializable value);

        Task<bool> DeleteAsync(int userId, string key);

    }

}
