using Plato.Internal.Stores.Abstractions;

namespace Plato.Mentions.Stores
{

    public interface IEntityMentionsStore<TModel> : IStore<TModel> where TModel : class
    {

    }

}
