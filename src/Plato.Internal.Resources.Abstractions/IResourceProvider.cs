using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plato.Internal.Resources.Abstractions
{

    public interface IResourceProvider
    {
        Task<IEnumerable<ResourceEnvironment>> GetResourceGroups();

    }

}
