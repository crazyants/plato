using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{

    public interface ICategoryRoleRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByCategoryIdAsync(int categoryId);

        Task<bool> DeleteByCategoryIdAsync(int categoryId);

    }


}
