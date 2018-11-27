using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Follow.Repositories
{
    public interface IFollowRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectFollowsByNameAndThingId(string name, int thingId);

        Task<TModel> SelectFollowsByCreatedUserIdAndThingId(int userId, int thingId);

    }

}
