using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Resources.Abstractions
{

    public interface IResourceProvider
    {
        IEnumerable<ResourceGroup> GetResourceGroups();

    }

}
