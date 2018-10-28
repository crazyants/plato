using Plato.Internal.Abstractions;

namespace Plato.Entities.Services
{
    public interface IEntityReplyManager<TModel> : ICommandManager<TModel> where TModel : class
    {

        event EntityReplyEvents<TModel>.Handler Creating;
        event EntityReplyEvents<TModel>.Handler Created;
        event EntityReplyEvents<TModel>.Handler Updating;
        event EntityReplyEvents<TModel>.Handler Updated;
        event EntityReplyEvents<TModel>.Handler Deleting;
        event EntityReplyEvents<TModel>.Handler Deleted;
        
    }
}
