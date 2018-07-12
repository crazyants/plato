using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Services
{
    public interface IEntityManager<TModel> where TModel : class
    {

        event EntityEvents<TModel>.Handler Creating;
        event EntityEvents<TModel>.Handler Created;
        event EntityEvents<TModel>.Handler Updating;
        event EntityEvents<TModel>.Handler Updated;
        event EntityEvents<TModel>.Handler Deleting;
        event EntityEvents<TModel>.Handler Deleted;
      
        Task<IActivityResult<TModel>> CreateAsync(TModel model);

        Task<IActivityResult<TModel>> UpdateAsync(TModel model);

        Task<IActivityResult<TModel>> DeleteAsync(int id);

    }
    
}
