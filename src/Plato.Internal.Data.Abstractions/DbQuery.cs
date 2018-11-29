using Microsoft.Extensions.Options;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbQueryConfiguration : IDbQueryConfiguration
    {
        
        private readonly DbContextOptions _dbContextOptionss;
        private readonly IFullTextQueryParser _fullTextQueryParser;

        public DbQueryConfiguration(
            IOptions<DbContextOptions> dbContextOptions,
            IFullTextQueryParser fullTextQueryParser)
        {
            _fullTextQueryParser = fullTextQueryParser;
            _dbContextOptionss = dbContextOptions.Value;
        }

        public IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class
        {
            query.Options.TablePrefix = _dbContextOptionss.TablePrefix;
            query.Options.FullTextQueryParser = _fullTextQueryParser;
            return query;
        }
    }

}
