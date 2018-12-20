using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.History.Stores
{
    public interface IFollowStore<TModel> : IStore<TModel> where TModel : class
    {


    }


}
