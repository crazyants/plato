using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Abstractions;

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
      
        Task<IActivityResult<TModel>> CreateAsync(TModel model);

        Task<IActivityResult<TModel>> UpdateAsync(TModel model);

        Task<IActivityResult<TModel>> DeleteAsync(int id);

    }
    
}
