using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Plato.Modules.Abstractions
{
    public interface IModuleLoader
    {
        List<Assembly> LoadModule(IModuleDescriptor descriptor);      
        
    }
}
