using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Data.Abstractions
{
    public class SearchQuery
    {

        public SearchTypes SearchType { get; private set; }

        public string Query { get; private set; }

        protected SearchQuery()
        {

        }

        public SearchQuery(SearchTypes searchType, string query)
        {
            SearchType = searchType;
            Query = query;
        }

    }
}
