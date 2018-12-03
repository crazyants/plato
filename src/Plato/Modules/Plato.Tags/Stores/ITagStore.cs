using Plato.Internal.Stores.Abstractions;

namespace Plato.Tags.Stores
{
    public interface ITagStore<TModel> : IStore<TModel> where TModel : class
    {

    }


}
