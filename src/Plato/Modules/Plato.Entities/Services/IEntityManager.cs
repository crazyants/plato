using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.Entities.Services
{
    public interface IEntityManager<in TModel> where TModel : class
    {

        event EntityEvents.EntityEventHandler Creating;
        event EntityEvents.EntityEventHandler Created;
        event EntityEvents.EntityEventHandler Updating;
        event EntityEvents.EntityEventHandler Updated;
        event EntityEvents.EntityEventHandler Deleting;
        event EntityEvents.EntityEventHandler Deleted;
      
        Task<IEntityResult> CreateAsync(TModel model);

        Task<IEntityResult> UpdateAsync(TModel model);

        Task<IEntityResult> DeleteAsync(int id);

    }


}
