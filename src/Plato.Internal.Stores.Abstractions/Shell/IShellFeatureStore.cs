using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions.Shell
{

    public interface IShellFeatureStore<T> : IStore2<T> where T : class
    {
        Task<IEnumerable<T>> SelectFeatures();
    }
    
}
