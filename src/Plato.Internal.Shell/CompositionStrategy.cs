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
                _logger.LogDebug($"Composing blueprint for tennet {settings.Name} ");
            }

            // Get all module names registered with the current shell
            var moduleNames = descriptor.Modules.Select(x => x.Name).ToArray();

            // Get modules from names
            var modules = await _moduleManager.LoadModulesAsync(moduleNames);

            // Get exported types for loaded modules
            var entries = new Dictionary<Type, IModuleEntry>();
            if (modules != null)
            {
                foreach (var module in modules)
                {
                    if (module.ExportedTypes != null)
                    {
                        foreach (var exportedType in module.ExportedTypes)
                        {
                            entries.Add(exportedType, module);
                        }
                    }
                }

            }

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
