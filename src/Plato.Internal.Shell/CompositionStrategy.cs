using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Shell.Models;

namespace Plato.Internal.Shell
{
    public class CompositionStrategy : ICompositionStrategy
    {
        private readonly IModuleManager _moduleManager;
        private readonly ILogger _logger;
  

        public CompositionStrategy(
            IModuleManager moduleManager,
            ILogger<CompositionStrategy> logger)
        {
            _moduleManager = moduleManager;
            _logger = logger;
        }

        public async Task<ShellBlueprint> ComposeAsync(ShellSettings settings, ShellDescriptor descriptor)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Composing blueprint");
            }

            var moduleNames = descriptor.Modules.Select(x => x.Name).ToArray();

            var modules = await _moduleManager.LoadModulesAsync(moduleNames);

            var entries = new Dictionary<Type, IModuleEntry>();

            foreach (var module in modules)
            {
                foreach (var exportedType in module.ExportedTypes)

                {
                    entries.Add(exportedType, module);
                }
            }

            //foreach (var feature in features)
            //{
            //    foreach (var exportedType in feature.ExportedTypes)
            //    {
            //        var requiredFeatures = RequireFeaturesAttribute.GetRequiredFeatureNamesForType(exportedType);

            //        if (requiredFeatures.All(x => featureNames.Contains(x)))
            //        {
            //            entries.Add(exportedType, feature);
            //        }
            //    }
            //}

            var result = new ShellBlueprint
            {
                Settings = settings,
                Descriptor = descriptor,
                Dependencies = entries
            };

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Done composing blueprint");
            }
            return result;
        }
    }
}
