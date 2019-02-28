using Plato.Internal.Models;

namespace Plato.Categories.Models
{
    public interface ICategoryMetaData<TModel> : IMetaData<TModel> where TModel : class
    {
    }
}
