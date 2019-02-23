using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Search.Models;

namespace Plato.Search.Repositories
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<Catalog>> SelectCatalogs();

    }

}
