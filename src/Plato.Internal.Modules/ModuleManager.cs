using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Plato.Internal.Modules.Abstractions;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Threading.Tasks;
using Plato.Internal.Modules.Models;

namespace Plato.Internal.Modules
{
    public class ModuleManager : IModuleManager
    {

        #region "Private Variables"

        readonly IModuleLocator _moduleLocator;
        readonly IModuleLoader _moduleLoader;       
        IEnumerable<IModuleDescriptor> _moduleDescriptors;

        static List<IModuleEntry> _moduleEntries;
        static List<Assembly> _loadedAssemblies;

        readonly string _contentRootPath;
        readonly string _virtualPathToModules;

        #endregion

        #region "Public ReadOnly Propertoes"

   

        public IEnumerable<IModuleEntry> AvailableModules => _moduleEntries;

        public IEnumerable<Assembly> AllAvailableAssemblies => _loadedAssemblies;

        #endregion

        #region "Constructor"

        public ModuleManager(
            IHostingEnvironment hostingEnvironment,
            IOptions<ModuleOptions> moduleOptions,
            IModuleLocator moduleLocator,
            IModuleLoader moduleLoader)
        {
            _moduleLocator = moduleLocator;
            _moduleLoader = moduleLoader;
            _contentRootPath = hostingEnvironment.ContentRootPath;
            _virtualPathToModules = moduleOptions.Value.VirtualPathToModulesFolder;
        }

        #endregion

        #region "Implementation"

        public async Task<IEnumerable<IModuleEntry>> LoadModulesAsync()
        {
            await InitializeModules();
            return _moduleEntries;
        }

        public async Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync()
        {
            await InitializeModules();
            return _loadedAssemblies;
        }

        public async Task<IEnumerable<Assembly>> LoadModuleAssembliesAsync(string[] moduleIds)
        {

            await InitializeModules();
     
            // Get descriptors for supplied moduleIds
            var descriptors = _moduleEntries
                .Where(e => moduleIds.Any(moduleId => moduleId.Equals(e.Descriptor.Id, StringComparison.OrdinalIgnoreCase)))
                .Select(d => d.Descriptor)
                .ToList();
          
            // Build all dependencies
            var loadedAssemblies = new List<Assembly>();
            foreach (var descriptor in descriptors)
            {
                var assemblies = await _moduleLoader.LoadModuleAsync(descriptor);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assmeblies = assemblies
                });
                loadedAssemblies.AddRange(assemblies);

            }

            return loadedAssemblies;

        }


        public async Task<IEnumerable<IModuleEntry>> LoadModulesAsync(string[] moduleIds)
        {
            await InitializeModules();

            var moduless = _moduleEntries.Select(m => m.Descriptor.Id).ToList();
            var loadedModules = _moduleEntries
                .Where(m => moduleIds.Contains(m.Descriptor.Id)).ToList();

            return loadedModules;
        }

        #endregion

        #region "Private Methods"

        async Task InitializeModules()
        {
            if (_moduleEntries == null)
            {
                _moduleEntries = new List<IModuleEntry>();
                _loadedAssemblies = new List<Assembly>();
                await LocateModuleDescriptors();
                await LoadModulesInternalAsync();
            }
        }
        
        async Task LocateModuleDescriptors()
        {
            _moduleDescriptors = await _moduleLocator.LocateModulesAsync(
                new string[] { _contentRootPath + "\\" + _virtualPathToModules },
                "Module", "module.txt", false);
        }

        async Task LoadModulesInternalAsync()
        {

            if (_moduleDescriptors == null)
                throw new NullReferenceException(nameof(_moduleDescriptors));

            foreach (var descriptor in _moduleDescriptors)
            {
                var assemblies = await _moduleLoader.LoadModuleAsync(descriptor);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assmeblies = assemblies
                });
                _loadedAssemblies.AddRange(assemblies);

            }

        }
        
        #endregion

    }
}
