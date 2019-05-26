using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Labels.Stores
{
    public interface ILabelDataStore<T> : IStore<T> where T : class
    {

        Task<IEnumerable<T>> GetByLabelIdAsync(int entityId);

    }

}
