using System.Threading.Tasks;

namespace Plato.Entities.Services
{
    public interface IEntityReplyManager<in TModel> where TModel : class
    {

        event EntityReplyEvents.Handler Creating;
        event EntityReplyEvents.Handler Created;
        event EntityReplyEvents.Handler Updating;
        event EntityReplyEvents.Handler Updated;
        event EntityReplyEvents.Handler Deleting;
        event EntityReplyEvents.Handler Deleted;

        Task<IEntityResult> CreateAsync(TModel reply);

        Task<IEntityResult> UpdateAsync(TModel reply);

        Task<IEntityResult> DeleteAsync(int id);

    }
}
