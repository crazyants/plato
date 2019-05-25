using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Labels.Stores
{
    public interface ILabelRoleStore<TModel> : IStore2<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByLabelIdAsync(int labelId);

        Task<bool> DeleteByLabelIdAsync(int labelId);
        
        Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int labelId);

    }


}
