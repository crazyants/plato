using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions
{

    public interface IFullTextQueryParser
    {

        HashSet<string> StopWords { get; set; }

        string ToFullTextSearchQuery(string query);

    }


}
