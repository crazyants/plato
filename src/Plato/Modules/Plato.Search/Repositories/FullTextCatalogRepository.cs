using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Search.Models;

namespace Plato.Search.Repositories
{
    
    public class FullTextCatalogRepository : IFullTextCatalogRepository
    {

        private const string BySql = "SELECT * FROM sys.fulltext_catalogs";

        private readonly IDbContext _dbContext;

        public FullTextCatalogRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FullTextCatalog>> SelectCatalogsAsync()
        {

            ICollection<FullTextCatalog> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.Text, BySql);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<FullTextCatalog>();
                    while (await reader.ReadAsync())
                    {
                        var catalog = new FullTextCatalog();
                        catalog.PopulateModel(reader);
                        output.Add(catalog);
                    }
                 
                }
            }

            return output;

        }

    }

}
