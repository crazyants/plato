using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions
{
    
    public interface IStore<TModel> : IQueryableStore<TModel> where TModel : class
    {

        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);

        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);

    }


}