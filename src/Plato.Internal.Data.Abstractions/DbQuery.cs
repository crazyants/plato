using Microsoft.Extensions.Options;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbQueryConfiguration : IDbQueryConfiguration
    {
        
        private readonly DbContextOptions _dbContextOptions;
   
        public DbQueryConfiguration(IOptions<DbContextOptions> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions.Value;
        }

        public IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class
        {
            query.Options.TablePrefix = _dbContextOptions.TablePrefix;
            return query;
        }

    }

}
