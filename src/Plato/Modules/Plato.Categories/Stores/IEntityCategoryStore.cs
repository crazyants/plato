using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface IEntityCategoryStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByEntityIdAsync(int entityId);

        Task<TModel> GetByEntityIdAndCategoryIdAsync(int entityId, int categoryId);
        
        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndCategoryIdAsync(int entityId, int categoryId);

    }

}
