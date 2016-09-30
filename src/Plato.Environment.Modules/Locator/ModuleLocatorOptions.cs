using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Environment.Modules
{
    public class ModuleLocatorOptions
    {

        public IList<IModuleLocatorExpander> ModuleLocatorExpanders{ get; }
            = new List<IModuleLocatorExpander>();

    }
}
