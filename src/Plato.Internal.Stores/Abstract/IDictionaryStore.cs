using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Stores.Abstract
{
    public interface IDictionaryStore
    {
        Task<T> GetAsync<T>(string key) where T : class;

        Task<T> UpdateAsync<T>(string key, ISerializable value) where T : class;

        Task<bool> DeleteAsync(string key);

    }
}
