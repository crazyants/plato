using System.Data;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Repositories
{
    
    public interface IRepository2<TModel> where TModel : class
    {
        Task<TModel> InsertUpdateAsync(TModel model);

        Task<TModel> SelectByIdAsync(int id);

        Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams);

        Task<bool> DeleteAsync(int id);

    }
    

}