using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Stores.Abstract
{
    public interface IDictionaryStore
    {
        Task<T> GetAsync<T>(string key);

        Task<T> UpdateAsync<T>(string key, ISerializable value);

        Task<bool> DeleteAsync(string key);

    }
}
