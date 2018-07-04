using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions
{

    public interface IStore<TModel> where TModel : class
    {
 
        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);
        
        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);
        
        IQuery<TModel> QueryAsync();

        Task<IPagedResults<TModel>> SelectAsync(params object[] args);
        
    }
}