using System.Collections.Generic;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions
{

    public interface IFederatedQueryManager<TModel> where TModel : class
    {
        IEnumerable<string> GetQueries(IQuery<TModel> query);
    }
    
}
