using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface ICategoryDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByCategoryIdAsync(int entityId);

    }

}
