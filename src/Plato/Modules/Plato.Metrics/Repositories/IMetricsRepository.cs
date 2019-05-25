using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Metrics.Repositories
{
    public interface IMetricsRepository<TModel> : IRepository2<TModel> where TModel : class
    {

    }

}