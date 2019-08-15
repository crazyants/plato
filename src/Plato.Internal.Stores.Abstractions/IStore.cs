using System.Threading.Tasks;

namespace Plato.Internal.Stores.Abstractions
{

    public interface ICacheableStore<TModel> where TModel : class
    {
        void CancelTokens(TModel model);
    }

    public interface IStore<TModel> :
        ICacheableStore<TModel>,
        IQueryableStore<TModel> where TModel : class
    {

        Task<TModel> CreateAsync(TModel model);

        Task<TModel> UpdateAsync(TModel model);

        Task<bool> DeleteAsync(TModel model);

        Task<TModel> GetByIdAsync(int id);
        
    }


}