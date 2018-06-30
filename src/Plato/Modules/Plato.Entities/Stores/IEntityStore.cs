using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    public interface IEntityStore<TModel> : IStore<TModel> where TModel : class
    {
      

    }


}
