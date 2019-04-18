using System.Collections.Generic;
using Plato.Internal.Search.Abstractions;

namespace Plato.Tags
{

    public class FullTextIndexes : IFullTextIndexProvider
    {

        public static readonly FullTextIndex Tags =
            new FullTextIndex("Tags", new string[] {"Name", "Description" });

        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Tags
            };
        }

    }

}
