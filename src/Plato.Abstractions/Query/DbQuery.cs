using Microsoft.Extensions.Options;
using Plato.Abstractions.Data;

namespace Plato.Abstractions.Query
{
    
    public class DbQuery : IDbQuery
    {
        private readonly DbContextOptions _dbContextOptionss;

        public DbQuery(IOptions<DbContextOptions> dbContextOptions)
        {
            _dbContextOptionss = dbContextOptions.Value;
        }

        public IQuery ConfigureQuery(IQuery query) 
        {
            query.TablePrefix = _dbContextOptionss.TablePrefix;
            return query;
        }

    }
}
