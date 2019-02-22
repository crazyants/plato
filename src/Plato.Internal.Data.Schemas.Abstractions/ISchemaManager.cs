using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Data.Schemas.Abstractions
{
    public interface ISchemaManager
    {
        Task<IEnumerable<string>> ExecuteAsync(IEnumerable<string> statements);
    }
    
}
