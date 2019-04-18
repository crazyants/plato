using System.Collections.Generic;

namespace Plato.Internal.Stores.Abstractions
{

    public interface IFederatedQueryManager<TModel> where TModel : class
    {
        IEnumerable<string> GetQueries(IFederatedQueryContext<TModel> context);
    }
    
}
