using System.Collections.Generic;

namespace Plato.Internal.Search.Abstractions
{
    public interface IFullTextIndexManager
    {

        IEnumerable<FullTextIndex> GetIndexes();

        IDictionary<string, IEnumerable<FullTextIndex>> GetIndexesByTable();

    }
    
}
