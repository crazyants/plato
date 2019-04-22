using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Metrics.Stores
{
    public interface IEntityMetricsStore<TModel> : IStore<TModel> where TModel : class
    {
  
    }

}
