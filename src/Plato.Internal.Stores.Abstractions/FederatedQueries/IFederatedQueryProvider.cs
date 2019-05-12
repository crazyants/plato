using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions.FederatedQueries
{
    public interface IFederatedQueryProvider<TModel> where TModel : class
    {
        IEnumerable<string> Build(IQuery<TModel> query);
    }
}
