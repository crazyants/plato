using System.Collections.Generic;

namespace Plato.Internal.Search.Abstractions
{

    public interface IFullTextIndexProvider
    {
        IEnumerable<FullTextIndex> GetIndexes();
    }
    
}
