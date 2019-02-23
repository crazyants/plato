using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Repositories
{
    
    public class CatalogRepository : ICatalogRepository
    {

        private readonly IDbContext _dbContext;

        public CatalogRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Catalog>> SelectCatalogs()
        {

            ICollection<Catalog> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.Text,
                    "SELECT * FROM sys.fulltext_catalogs");
                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<Catalog>();
                    while (await reader.ReadAsync())
                    {
                        var catalog = new Catalog();
                        catalog.PopulateModel(reader);
                        output.Add(catalog);
                    }
                 
                }
            }

            return output;

        }

    }

}
