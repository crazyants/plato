using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Abstractions.Stores
{
    public interface IStore<TModel> where TModel : class
    {
        Task<IEnumerable<TModel>> GetAsync(int pageIndex, int pageSize, params object[] args);

        Task<TModel> GetAsync(TModel model);

        Task<TModel> SaveAsync(TModel model);

        Task<bool> DeleteAsync(TModel model);
    }
}