using Plato.Internal.Models;

namespace Plato.Entities.Models
{
    public interface IEntityMetaData<TModel> : IMetaData<TModel> where TModel : class
    {
    }

}
