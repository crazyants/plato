using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{
    public interface IEntityCategoryRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityIdAsync(int entityId);

        Task<T> SelectByEntityIdAndCategoryIdAsync(int entityId, int categoryId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndCategoryIdAsync(int entityId, int categoryId);

    }


}
