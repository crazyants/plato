using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Search.Abstractions
{
    public interface IFullTextIndexManager
    {

        Task<IEnumerable<string>> CreateIndexesAsync();

    }


}
