using System.Collections.Generic;

namespace Plato.Search.Models
{
    public static class SearchDefaults
    {

        public static IDictionary<int, SearchMethod> AvailableSearchMethods = new Dictionary<int, SearchMethod>()
        {
            {
                1, new SearchMethod()
                {
                    Name = "Tsql"
                }
            },
            {
                2, new SearchMethod()
                {
                    Name = "ContainsTable"
                }
            },
            {
                3, new SearchMethod()
                {
                    Name = "FreeTextTable"
                }
            }
        };
        
    }
    
    public class SearchMethod
    {

        public string Name { get; set; }
        
    }

}
