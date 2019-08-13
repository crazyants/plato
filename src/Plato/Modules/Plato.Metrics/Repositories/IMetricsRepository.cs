using Plato.Internal.Repositories;

namespace Plato.Metrics.Repositories
{
    public interface IMetricsRepository<TModel> : IRepository<TModel> where TModel : class
    {

    }

}