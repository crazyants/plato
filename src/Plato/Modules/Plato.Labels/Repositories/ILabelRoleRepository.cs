using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Labels.Repositories
{

    public interface ILabelRoleRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByLabelIdAsync(int labelId);

        Task<bool> DeleteByLabelIdAsync(int labelId);

        Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int labelId);

    }


}
