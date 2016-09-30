using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public class ModuleLocatorOptionsSetUp :
          ConfigureOptions<ModuleLocatorOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ExtensionHarvestingOptions"/>.
        /// </summary>
        public ModuleLocatorOptionsSetUp()
            : base(options => { })
        {
        }
    }
}
