using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Labels.Repositories
{

    public interface ILabelRoleRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByLabelIdAsync(int LabelId);

        Task<bool> DeleteByLabelIdAsync(int LabelId);

        Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int LabelId);

    }


}
