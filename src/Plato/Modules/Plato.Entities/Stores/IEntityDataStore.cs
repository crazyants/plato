using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Stores
{
    public interface IEntityDataStore<T> where T : class
    {

        Task<T> GetAsync<T>(int userId, string key);

        Task<T> UpdateAsync<T>(int userId, string key, ISerializable value);

        Task<bool> DeleteAsync(int userId, string key);

    }

}
