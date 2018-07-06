using System.Threading.Tasks;

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

        Task<IEntityResult<TModel>> CreateAsync(TModel reply);

        Task<IEntityResult<TModel>> UpdateAsync(TModel reply);

        Task<IEntityResult<TModel>> DeleteAsync(int id);

    }
}
