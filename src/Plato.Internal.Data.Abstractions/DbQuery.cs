using Microsoft.Extensions.Options;

namespace Plato.Internal.Data.Abstractions
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
