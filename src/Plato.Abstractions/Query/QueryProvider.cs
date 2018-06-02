using Microsoft.Extensions.Options;
using Plato.Abstractions.Query;
using Plato.Abstractions.Shell;

namespace Plato.Abstractions.Query
{

    public interface IQueryFacade
    {
        IQuery ConfigureQuery(IQuery query);
    }

    public class QueryFacade : IQueryFacade
    {
        private readonly IDbQueryOptions _queryOptions;

        public QueryFacade(
            IOptions<DbQueryOptions> queryOptions)
        {
            _queryOptions = queryOptions.Value;
        }

        public IQuery ConfigureQuery(IQuery query) 
        {
            query.TablePrefix = _queryOptions.TablePrefix;
            return query;

        }

    }
}
