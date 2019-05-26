using Plato.Internal.Stores.Abstractions;

namespace Plato.Moderation.Stores
{

    public interface IModeratorStore<TModel> : IStore2<TModel> where TModel : class
    {

    }

}
