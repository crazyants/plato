using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Abstractions
{
    public interface IDbHelper
    {

        Task<T> ExecuteScalarAsync<T>(string sql);

        Task<T> ExecuteScalarAsync<T>(string sql, IDictionary<string, string> replacements);
        
    }

}
