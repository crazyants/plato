using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Users
{
    public interface IUserDataStore<T> : IStore2<T> where T : class
    {

        Task<T> GetByKeyAndUserIdAsync(string key, int userId);

        Task<IEnumerable<T>> GetByUserIdAsync(int userId);

    }

}
