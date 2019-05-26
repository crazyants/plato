using Plato.Internal.Stores.Abstractions;

namespace Plato.Media.Stores
{
    public interface IMediaStore<TModel> : IStore<TModel> where TModel : class
    {

    }

}
