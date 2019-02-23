using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Search.Models;

namespace Plato.Search.Repositories
{
    public interface IFullTextCatalogRepository
    {
        Task<IEnumerable<FullTextCatalog>> SelectCatalogsAsync();

    }

}
