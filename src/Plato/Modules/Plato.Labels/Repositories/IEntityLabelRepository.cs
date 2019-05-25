using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Labels.Repositories
{
    public interface IEntityLabelRepository<T> : IRepository2<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAsync(int entityId);

        Task<bool> DeleteByEntityIdAndLabelIdAsync(int entityId, int labelId);

    }


}
