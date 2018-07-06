using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public interface IEntityManager<TModel> where TModel : class
    {

        event EntityEvents.Handler Creating;
        event EntityEvents.Handler Created;
        event EntityEvents.Handler Updating;
        event EntityEvents.Handler Updated;
        event EntityEvents.Handler Deleting;
        event EntityEvents.Handler Deleted;
      
        Task<IEntityResult<TModel>> CreateAsync(TModel model);

        Task<IEntityResult<TModel>> UpdateAsync(TModel model);

        Task<IEntityResult<TModel>> DeleteAsync(int id);

    }
    
}
