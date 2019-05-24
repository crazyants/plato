using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{

    public interface IRepository<TModel> where TModel : class
    {
        Task<TModel> InsertUpdateAsync(TModel model);

        Task<TModel> SelectByIdAsync(int id);

        Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams);

        //Task<IPagedResults<TModel>> SelectAsync2(DbParam[] dbParams);

        Task<bool> DeleteAsync(int id);
        
    }

}