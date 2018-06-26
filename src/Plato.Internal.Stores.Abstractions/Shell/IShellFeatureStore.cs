using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Internal.Stores.Abstractions.Shell
{

    public interface IShellFeatureStore<T> : IStore<T> where T : class
    {
        Task<IEnumerable<T>> SelectFeatures();
    }
    
}
