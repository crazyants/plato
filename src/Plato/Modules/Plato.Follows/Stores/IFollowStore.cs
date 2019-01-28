using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Follows.Stores
{
    public interface IFollowStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByNameAndThingId(string name, int thingId);

        Task<TModel> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId);

    }


}
