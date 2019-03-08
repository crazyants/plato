using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Labels.Stores
{
    public interface ILabelRoleStore<TModel> : IStore<TModel> where TModel : class
    {

        Task<IEnumerable<TModel>> GetByLabelIdAsync(int LabelId);

        Task<bool> DeleteByLabelIdAsync(int LabelId);
        
        Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int LabelId);

    }


}
