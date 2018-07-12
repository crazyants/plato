using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Stores.Users
{
    public interface IUserDataItemStore<TModel> where TModel : class
    {

        Task<T> GetAsync<T>(int userId, string key) where T : class;

        Task<T> UpdateAsync<T>(int userId, string key, ISerializable value) where T : class;

        Task<bool> DeleteAsync(int userId, string key);

    }

}
