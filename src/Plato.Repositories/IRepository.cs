using System.Threading.Tasks;
using Plato.Abstractions.Collections;

namespace Plato.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertUpdateAsync(T entity);

        Task<T> SelectByIdAsync(int id);

        Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class;

        Task<bool> DeleteAsync(int id);
    }
}