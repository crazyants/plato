using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;

namespace Plato.Internal.Stores.Abstractions
{

    public interface IQueryable<TModel> where TModel : class
    {
        IQuery<TModel> QueryAsync();

        Task<IPagedResults<TModel>> SelectAsync(params object[] args);
    }

    public interface IStore<TModel> : IQueryable<TModel> where TModel : class
    {
 
        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);
        
        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);
        
    }
}