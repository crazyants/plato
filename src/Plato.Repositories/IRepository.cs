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

        Task<IEnumerable<T>> SelectAsync(IQueryBuilder queryBuilder);

        Task<IEnumerable<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class;
        
        Task<bool> DeleteAsync(int id);

    }

}
