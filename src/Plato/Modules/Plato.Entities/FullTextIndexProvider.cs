using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Search.Abstractions;

namespace Plato.Entities
{
    public class FullTextIndexProvider : IFullTextIndexProvider
    {

        public FullTextIndexProvider()
        {

        }

        public Task<IEnumerable<string>> CreateIndexAsync()
        {
            throw new NotImplementedException();
        }

    }
}
