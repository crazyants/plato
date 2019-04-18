using System.Collections.Generic;
using Plato.Internal.Search.Abstractions;

namespace Plato.Labels
{

    public class FullTextIndexes : IFullTextIndexProvider
    {

        public static readonly FullTextIndex Labels =
            new FullTextIndex("Labels", new string[] {"[Name]", "[Description]" });

        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Labels
            };
        }

    }

}
