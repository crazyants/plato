using Microsoft.Extensions.Options;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbQueryConfiguration : IDbQueryConfiguration
    {
        private readonly DbContextOptions _dbContextOptionss;

        public DbQueryConfiguration(IOptions<DbContextOptions> dbContextOptions)
        {
            _dbContextOptionss = dbContextOptions.Value;
        }

        public IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class
        {
            query.TablePrefix = _dbContextOptionss.TablePrefix;
            return query;
        }
    }

}
