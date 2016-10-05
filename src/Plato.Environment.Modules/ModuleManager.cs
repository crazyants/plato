using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Plato.Environment.Modules.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using Plato.Environment.Modules.Abstractions.Features;

namespace Plato.Environment.Modules
{
    public class ModuleManager : IModuleManager
    {

        #region "Private Variables"

        IModuleLocator _moduleLocator;
        IModuleLoader _moduleLoader;       
        IEnumerable<IModuleDescriptor> _moduleDescriptors;
        static List<IModuleEntry> _moduleEntries;

        string _contentRootPath;       
        string _virtualPathToModules;

        #endregion

        #region "Public ReadOnly Propertoes"

        public IEnumerable<IModuleEntry> ModuleEntries
        {
            get {
                InitializeModules();
                return _moduleEntries;
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

       void InitializeModules()
        {
            if (_moduleEntries == null)
            {
                _moduleEntries = new List<IModuleEntry>();
                LoadModuleDescriptors();
                LoadModules();
            }
        }

        public void LoadModuleDescriptors()
        {
            _moduleDescriptors = _moduleLocator.LocateModules(
             new string[] { _contentRootPath + "\\" + _virtualPathToModules },
             "Module", "module.txt", false);
        }

        public void LoadModules()
        {

            if (_moduleDescriptors == null)
                throw new NullReferenceException(nameof(_moduleDescriptors));
                      
            foreach (var descriptor in _moduleDescriptors)
            {

                List<Assembly> assemblies = _moduleLoader.LoadModule(descriptor);
                _moduleEntries.Add(new ModuleEntry()
                {
                    Descriptor = descriptor,
                    Assmeblies = assemblies              
                    
                });

            }

        }

        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
