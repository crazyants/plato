using Microsoft.Extensions.Options;

namespace Plato.Internal.Data.Abstractions
{
    
    public class DbQueryConfiguration : IDbQueryConfiguration
    {
        
        private readonly DbContextOptions _dbContextOptionss;
        //private readonly IFederatedQueryManager _federatedQueryManager;

        public DbQueryConfiguration(
            IOptions<DbContextOptions> dbContextOptions
            //IFullTextQueryParser fullTextQueryParser
            )
        {
            //_fullTextQueryParser = fullTextQueryParser;
            _dbContextOptionss = dbContextOptions.Value;
            //_federatedQueryManager = federatedQueryManager;
        }

        public IQuery<T> ConfigureQuery<T>(IQuery<T> query) where T : class
        {
            query.Options.TablePrefix = _dbContextOptionss.TablePrefix;
            //query.FederatedQueryManager = _federatedQueryManager;

            //query.Options.FullTextQueryParser = _fullTextQueryParser;
            return query;
        }
    }

}
