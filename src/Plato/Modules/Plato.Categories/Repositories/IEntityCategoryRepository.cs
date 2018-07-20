using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{
    public interface IEntityCategoryRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityId(int entityId);

        Task<bool> DeleteByEntityId(int entityId);

        Task<bool> DeleteByEntityIdAndCategoryId(int entityId, int categoryId);

    }


}
