using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Services
{
    public interface IEntityReplyManager<TModel> where TModel : class
    {

        event EntityReplyEvents<TModel>.Handler Creating;
        event EntityReplyEvents<TModel>.Handler Created;
        event EntityReplyEvents<TModel>.Handler Updating;
        event EntityReplyEvents<TModel>.Handler Updated;
        event EntityReplyEvents<TModel>.Handler Deleting;
        event EntityReplyEvents<TModel>.Handler Deleted;

        Task<IActivityResult<TModel>> CreateAsync(TModel reply);

        Task<IActivityResult<TModel>> UpdateAsync(TModel reply);

        Task<IActivityResult<TModel>> DeleteAsync(int id);
        
    }
}
