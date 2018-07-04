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


    public class DbQuery2 : IDbQuery2
    {
        private readonly DbContextOptions _dbContextOptionss;

        public DbQuery2(IOptions<DbContextOptions> dbContextOptions)
        {
            _dbContextOptionss = dbContextOptions.Value;
        }

        public IQuery ConfigureQuery(IQuery query)
        {
            query.TablePrefix = _dbContextOptionss.TablePrefix;
            return query;
        }

        public IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class
        {
            query.TablePrefix = _dbContextOptionss.TablePrefix;
            return query;
        }
    }

}
