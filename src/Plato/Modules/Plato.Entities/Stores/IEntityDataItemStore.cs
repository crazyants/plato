using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    public interface IEntityDataItemStore<T> where T : class
    {

        Task<T> GetAsync(int entityId, string key);

        Task<T> UpdateAsync(int entityId, string key, ISerializable value);

        Task<bool> DeleteAsync(int entityId, string key);

    }

}
