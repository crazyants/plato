using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{

    //public interface IRepository<TModel> where TModel : class
    //{
    //    Task<TModel> InsertUpdateAsync(TModel model);

    //    Task<TModel> SelectByIdAsync(int id);

    //    Task<IPagedResults<TModel>> SelectAsync(params object[] inputParams);
        
    //    Task<bool> DeleteAsync(int id);
        
    //}



    public interface IRepository2<TModel> where TModel : class
    {
        Task<TModel> InsertUpdateAsync(TModel model);

        Task<TModel> SelectByIdAsync(int id);

        Task<IPagedResults<TModel>> SelectAsync(DbParam[] dbParams);

        Task<bool> DeleteAsync(int id);

    }
    

}