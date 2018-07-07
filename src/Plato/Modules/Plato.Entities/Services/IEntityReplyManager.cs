using System.Threading.Tasks;
using Plato.Internal.Abstractions;

namespace Plato.Entities.Services
{
    public interface IEntityReplyManager<TModel> where TModel : class
    {

        event EntityReplyEvents.Handler Creating;
        event EntityReplyEvents.Handler Created;
        event EntityReplyEvents.Handler Updating;
        event EntityReplyEvents.Handler Updated;
        event EntityReplyEvents.Handler Deleting;
        event EntityReplyEvents.Handler Deleted;

        Task<IActivityResult<TModel>> CreateAsync(TModel reply);

        Task<IActivityResult<TModel>> UpdateAsync(TModel reply);

        Task<IActivityResult<TModel>> DeleteAsync(int id);

    }
}
