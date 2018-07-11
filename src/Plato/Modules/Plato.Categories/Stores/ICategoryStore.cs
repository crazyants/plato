using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{

    public interface ICategoryStore<TModel> : IStore<TModel> where TModel : class
    {

    }
    
}
