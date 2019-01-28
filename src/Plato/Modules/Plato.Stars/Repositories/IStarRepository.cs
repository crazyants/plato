using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Stars.Repositories
{
    public interface IStarRepository<TModel> : IRepository<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> SelectByNameAndThingId(string name, int thingId);

        Task<TModel> SelectByNameThingIdAndCreatedUserId(string name,  int thingId, int userId);

    }

}
