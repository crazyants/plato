using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Repositories.Shell
{
    public interface IShellFeatureRepository<T> : IRepository<T> where T : class
    {

        Task<IEnumerable<T>> SelectFeatures();
    }

}
