using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Plato.Modules.Abstractions;

namespace Plato.Modules
{

    public class TypedModuleProvider : ITypedModuleProvider
    {

        private readonly ConcurrentDictionary<Type, IModuleEntry> _modules
            = new ConcurrentDictionary<Type, IModuleEntry>();

        private readonly IModuleManager _moduleManager;

        public TypedModuleProvider(
            IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
          
        }
        
        public async Task<IModuleEntry> GetModuleForDependency(Type dependency)
        {

            await BuildTypedProvider();

            if (_modules.TryGetValue(dependency, out var module))
            {
                return module;
            }

            throw new InvalidOperationException($"Could not resolve module for type {dependency.Name}");
        }

        private async Task BuildTypedProvider()
        {
            var moduleEntries = await _moduleManager.LoadModulesAsync();
            foreach (var moduleEntry in moduleEntries)
            {
                // Get all valid types from module
                var types = moduleEntry.Assmeblies.SelectMany(assembly =>
                    assembly.ExportedTypes.Where(IsComponentType));
                foreach (var type in types)
                {
                    _modules.TryAdd(type, moduleEntry);
                }
            }
        }

        private bool IsComponentType(Type type)
        {
            if (type == null)
                return false;
            return type.IsClass && !type.IsAbstract && type.IsPublic;
        }

        public void TryAdd(Type type, IModuleEntry module)
        {
            _modules.TryAdd(type, module);
        }

    }
}
