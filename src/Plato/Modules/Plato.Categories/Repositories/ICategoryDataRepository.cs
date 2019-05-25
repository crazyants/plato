using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Categories.Repositories
{
    public interface ICategoryDataRepository<T> : IRepository2<T> where T : class
    {

        Task<IEnumerable<T>> SelectByCategoryIdAsync(int userId);

    }

}
