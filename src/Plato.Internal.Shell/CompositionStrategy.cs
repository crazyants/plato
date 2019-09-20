using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Modules;
using Plato.Internal.Models.Shell;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Shell
{
    public class CompositionStrategy : ICompositionStrategy
    {

        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly IModuleManager _moduleManager;
        private readonly ILogger _logger;
  
        public CompositionStrategy(
            IModuleManager moduleManager,
            ILogger<CompositionStrategy> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _typedModuleProvider = typedModuleProvider;
            _moduleManager = moduleManager;
            _logger = logger;            
        }

        public async Task<ShellBlueprint> ComposeAsync(IShellSettings settings, IShellDescriptor descriptor)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Composing blueprint for tennet {settings.Name} ");
            }

            // Get all module names registered with the current tennet
            var moduleNames = descriptor.Modules.Select(x => x.ModuleId).ToArray();

            // Get module entries for active modules
            var modules = await _moduleManager.LoadModulesAsync(moduleNames);
           
            //// Get all dependencies for loaded modules
            var entries = new Dictionary<Type, IModuleEntry>();
            if (modules != null)
            {
                foreach (var module in modules)
                {
                    var types = module.Assemblies.SelectMany(assembly =>
                        assembly.ExportedTypes.Where(IsComponentType));
                    foreach (var type in types)
                    {
                        if (!entries.ContainsKey(type))
                        {
                            entries.Add(type, module);
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

        private bool IsComponentType(Type type)
        {
            if (type == null)
                return false;
            return type.IsClass && !type.IsAbstract && type.IsPublic;
        }

    }

}
