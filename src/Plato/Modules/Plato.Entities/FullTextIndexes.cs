using System.Collections.Generic;
using Plato.Internal.Search.Abstractions;

namespace Plato.Entities
{
    public class FullTextIndexes : IFullTextIndexProvider
    {

        public static readonly FullTextIndex Entities =
            new FullTextIndex("Entities", new string[] {"Title", "Message", "Html", "Abstract"});

        public static readonly FullTextIndex EntityReplies =
            new FullTextIndex("EntityReplies", new string[] {"Message", "Html", "Abstract"});
        
        public IEnumerable<FullTextIndex> GetIndexes()
        {
            return new[]
            {
                Entities,
                EntityReplies
            };
        }

    }

}
