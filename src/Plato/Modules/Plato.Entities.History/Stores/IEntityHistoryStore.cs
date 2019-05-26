using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.History.Stores
{
    public interface IEntityHistoryStore<TModel> : IStore<TModel> where TModel : class
    {


    }


}
