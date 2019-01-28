using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Stars.Repositories
{
    public interface IStarRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectFollowsByNameAndThingId(string name, int thingId);

        Task<TModel> SelectFollowByNameThingIdAndCreatedUserId(string name,  int thingId, int userId);

    }

}
