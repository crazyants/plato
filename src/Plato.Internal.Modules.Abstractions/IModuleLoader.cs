using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plato.Internal.Modules.Abstractions
{
    public interface IModuleLoader
    {
        Task<List<Assembly>> LoadModuleAsync(IModuleDescriptor descriptor);      
        
    }
}
