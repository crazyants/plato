using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Internal.Resources.Abstractions
{
    public interface IResourceManager
    {

        Task<IEnumerable<ResourceGroup>> GetResources();

    }


}
