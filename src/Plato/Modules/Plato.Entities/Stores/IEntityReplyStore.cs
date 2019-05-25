using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityReplyStore<TModel> : IStore2<TModel> where TModel : class
    {
    }
    
}
