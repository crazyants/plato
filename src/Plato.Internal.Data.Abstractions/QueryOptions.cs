namespace Plato.Internal.Data.Abstractions
{
    public enum SearchTypes
    {
        Tsql = 0,
        ContainsTable = 1,
        FreeTextTable = 2
    }
    
    public class QueryOptions
    {

        public int MaxResults { get; set; }

        public SearchTypes SearchType { get; set; } = SearchTypes.Tsql;

    }

}
