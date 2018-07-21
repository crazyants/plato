using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Repositories;

namespace Plato.Labels.Repositories
{
    public interface ILabelDataRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectByLabelIdAsync(int userId);

    }

}
