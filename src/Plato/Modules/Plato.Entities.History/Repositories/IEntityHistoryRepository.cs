using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.History.Repositories
{
    public interface IEntityHistoryRepository<TModel> : IRepository2<TModel> where TModel : class
    {
        
    }

}
