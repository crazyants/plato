using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Environment.Modules
{
    public class ModuleLocatorAccessor : IModuleLocatorAccessor
    {
        private readonly IOptions<ModuleLocatorOptions> _optionsAccessor;
        private readonly IModuleLocator _moduleLocator;

        public ModuleLocatorAccessor(
            IOptions<ModuleLocatorOptions> optionsAccessor,
            IModuleLocator moduleLocator)
        {
            _optionsAccessor = optionsAccessor;
            _moduleLocator = moduleLocator;
        }

        public IEnumerable<ModuleDescriptor> LocateModules()
        {
            return _optionsAccessor.Value.ModuleLocatorExpanders
                .SelectMany(x => _moduleLocator.LocateModules(
                    x.SearchPaths, x.ModuleType, x.ManifestName, x.ManifestOptional));
        }
    }
}
