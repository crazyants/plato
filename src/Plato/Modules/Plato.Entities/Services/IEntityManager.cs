using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public interface IEntityManager<in TModel> where TModel : class
    {

        event EntityEvents.Handler Creating;
        event EntityEvents.Handler Created;
        event EntityEvents.Handler Updating;
        event EntityEvents.Handler Updated;
        event EntityEvents.Handler Deleting;
        event EntityEvents.Handler Deleted;
      
        Task<IEntityResult> CreateAsync(TModel model);

        Task<IEntityResult> UpdateAsync(TModel model);

        Task<IEntityResult> DeleteAsync(int id);

    }
    
}
