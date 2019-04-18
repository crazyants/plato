using System.Collections.Generic;

namespace Plato.Internal.Stores.Abstractions
{
    public interface IFederatedQueryProvider<TModel> where TModel : class
    {
        IEnumerable<string> GetQueries(IFederatedQueryContext<TModel> context);

    }
}
