using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Stars.Stores
{
    public interface IStarStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectFollowsByNameAndThingId(string name, int thingId);

        Task<TModel> SelectFollowByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId);

    }


}
