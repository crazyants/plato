using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.Settings
{
    public interface IDictionaryFactory
    {
        Task<T> GetAsync<T>(string key);

        Task<T> UpdateAsync<T>(string key, ISerializable value);

        Task<bool> DeleteByKeyAsync(string key);

    }
}
