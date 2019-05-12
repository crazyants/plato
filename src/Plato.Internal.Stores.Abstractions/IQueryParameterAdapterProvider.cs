using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions
{


    public interface IQueryAdapterManager<in TParams> where TParams : class
    {

        IEnumerable<string> GetAdaptations(TParams queryParams);

    }

    public interface IQueryAdapterProvider<in TParams> where TParams : class
    {

        string AdaptQuery(TParams queryParams);

    }
}
