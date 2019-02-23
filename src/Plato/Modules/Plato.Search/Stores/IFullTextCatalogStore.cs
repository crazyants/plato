using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Search.Models;

namespace Plato.Search.Stores
{
    public interface IFullTextCatalogStore
    {
        Task<IEnumerable<FullTextCatalog>> SelectCatalogsAsync();
    }

}
