using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    public interface IEntityStore<TModel> : IStore<TModel> where TModel : class
    {

    }


}
