using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{
    public interface IEntityDataRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectByEntityIdAsync(int userId);

    }

}
