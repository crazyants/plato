using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Plato.Modules.Abstractions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Plato.Modules
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

        public IEnumerable<IModuleEntry> AvailableModules
        {
            get
            {
                InitializeModules();
                return _moduleEntries;
            }
        }

        public IEnumerable<Assembly> AllAvailableAssemblies
        {
            get
            {
                InitializeModules();
                return _loadedAssemblies;
            }
        }


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
            _virtualPathToModules = moduleOptions.Value.VirtualPathToModules;            
            InitializeModules();

        }

        #endregion

        #region "Implementation"

        public void LoadModules()
        {

            if (_moduleDescriptors == null)
                throw new NullReferenceException(nameof(_moduleDescriptors));
                      
            foreach (var descriptor in _moduleDescriptors)
            {
                var assemblies = _moduleLoader.LoadModule(descriptor);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assmeblies = assemblies                                  
                });
                _loadedAssemblies.AddRange(assemblies);
                
            }

        }

        #endregion

        #region "Private Methods"
        
        void InitializeModules()
        {
            if (_moduleEntries == null)
            {
                _moduleEntries = new List<IModuleEntry>();
                _loadedAssemblies = new List<Assembly>();
                LoadModuleDescriptors();
                LoadModules();
            }
        }

        void LoadModuleDescriptors()
        {
            _moduleDescriptors = _moduleLocator.LocateModules(
             new string[] { _contentRootPath + "\\" + _virtualPathToModules },
             "Module", "module.txt", false);
        }

        #endregion

    }
}
