using System.Collections.Generic;

namespace Plato.Search.Models
{
    public static class SearchDefaults
    {
        
        private const string TsqlId = "Tsql";
        private const string ContainsTableId = "ContainsTable";
        private const string FreeTextTableId = "FreeTextTable";

        private const string TsqlName = "Standard (Tsql)";
        private const string ContainsTableName = "Full Text (ContainsTable)";
        private const string FreeTextTableName = "Full Text  (FreeTextTable)";

        public static IEnumerable<SearchMethod> AvailableSearchMethods = new List<SearchMethod>()
        {
            new SearchMethod(TsqlId, TsqlName),
            new SearchMethod(ContainsTableId, ContainsTableName),
            new SearchMethod(FreeTextTableId, FreeTextTableName)
        };

    }
    
    public class SearchMethod
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public SearchMethod()
        {
        }

        public SearchMethod(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
