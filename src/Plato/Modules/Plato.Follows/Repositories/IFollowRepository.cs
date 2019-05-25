using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Follows.Repositories
{
    public interface IFollowRepository<TModel> : IRepository2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByNameAndThingId(string name, int thingId);

        Task<TModel> SelectByNameThingIdAndCreatedUserId(string name,  int thingId, int userId);

    }

}
