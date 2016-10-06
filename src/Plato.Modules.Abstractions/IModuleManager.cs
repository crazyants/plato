using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Environment.Modules.Abstractions.Features;

namespace Plato.Environment.Modules.Abstractions
{
    public interface IModuleManager
    {

        IEnumerable<FeatureDescriptor> AvailableFeatures();

        IEnumerable<IModuleEntry> ModuleEntries { get; }

        void LoadModuleDescriptors();

        void LoadModules();

    }
}
