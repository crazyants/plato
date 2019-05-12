using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions.QueryAdapters
{



    public interface IQueryAdapterProvider<TModel> where TModel : class
    {

        string AdaptQuery(IQuery<TModel> query);

    }
}
