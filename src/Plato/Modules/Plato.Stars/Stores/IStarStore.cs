using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Stars.Stores
{
    public interface IStarStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByNameAndThingId(string name, int thingId);

        Task<TModel> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId);

    }


}
