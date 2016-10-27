using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Plato.Data.Query;

namespace Plato.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertUpdateAsync(T entity);

        Task<T> SelectByIdAsync(int id);
 
        Task<IEnumerable<T>> SelectPagedAsync(int pageIndex, int pageSize, object args);

        Task<IEnumerable<T>> Select(object args);

        Task<bool> DeleteAsync(int id);

    }

}
