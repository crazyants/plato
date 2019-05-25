using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Categories.Stores
{
    public interface ICategoryRoleStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByCategoryIdAsync(int categoryId);

        Task<bool> DeleteByCategoryIdAsync(int categoryId);
        
        Task<bool> DeleteByRoleIdAndCategoryIdAsync(int roleId, int categoryId);

    }


}
