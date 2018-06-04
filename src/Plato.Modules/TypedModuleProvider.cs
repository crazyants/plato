using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            BuildTypedProvider();
        }
        
        public IModuleEntry GetModuleForDependency(Type dependency)
        {
            if (_modules.TryGetValue(dependency, out var module))
            {
                return module;
            }

            throw new InvalidOperationException($"Could not resolve module for type {dependency.Name}");
        }

        private void BuildTypedProvider()
        {
            var moduleEntries = _moduleManager.AvailableModules.ToList();
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
