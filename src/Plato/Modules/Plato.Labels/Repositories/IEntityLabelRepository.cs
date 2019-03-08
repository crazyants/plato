using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Labels.Repositories
{
    public interface IEntityLabelRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectByEntityId(int entityId);

        Task<bool> DeleteByEntityId(int entityId);

        Task<bool> DeleteByEntityIdAndLabelId(int entityId, int labelId);

    }


}
