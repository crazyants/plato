using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> InsertUpdateAsync(T entity);

        Task<T> SelectByIdAsync(int id);

        Task<IPagedResults<TModel>> SelectAsync<TModel>(params object[] inputParams) where TModel : class;

        Task<IPagedResults<T>> SelectAsync(params object[] inputParams);
        
        Task<bool> DeleteAsync(int id);



    }
}