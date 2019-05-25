using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions
{
    
    /// <summary>
    /// Represents a store that supports all basic CRUD operations.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IStore<TModel> : IQueryableStore<TModel> where TModel : class
    {
 
        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);
        
        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);
        
    }


    public interface IStore2<TModel> : IQueryableStore2<TModel> where TModel : class
    {

        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);

        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);

    }


}