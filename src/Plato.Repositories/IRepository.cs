using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertUpdateAsync(T entity);

        Task<T> SelectByIdAsync(int id);
 
        Task<IEnumerable<T>> SelectPagedAsync(int pageIndex, int pageSize, object options);

        Task<bool> DeleteAsync(int id);
    }
}
