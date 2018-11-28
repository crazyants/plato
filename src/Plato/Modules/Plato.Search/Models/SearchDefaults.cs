using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;

namespace Plato.Search.Models
{
    
    public static class SearchDefaults
    {
        
        private const string TsqlName = "Standard (Tsql)";
        private const string ContainsTableName = "Full Text (ContainsTable)";
        private const string FreeTextTableName = "Full Text  (FreeTextTable)";

        public static IEnumerable<SearchType> AvailableSearchTypes = new List<SearchType>()
        {
            new SearchType(SearchTypes.Tsql, TsqlName),
            new SearchType(SearchTypes.ContainsTable, ContainsTableName),
            new SearchType(SearchTypes.FreeTextTable, FreeTextTableName)
        };

    }
    
    public class SearchType
    {

        public SearchTypes Type { get; set; }

        public string Name { get; set; }

        protected SearchType()
        {
        }

        public SearchType(SearchTypes type, string name)
        {
            Type = type;
            Name = name;
        }
    }

}
