using Plato.Internal.Stores.Abstractions;

namespace Plato.Media.Stores
{
    public interface IMediaStore<TModel> : IStore2<TModel> where TModel : class
    {

    }

}
