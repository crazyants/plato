using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using System.IO;
using Plato.FileSystem;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Runtime.Loader;

namespace Plato.Environment.Modules
{
    public class ModuleLoaderService : IModuleLoaderService
    {

        #region "Private Variables"

        private static readonly ConcurrentDictionary<string, Lazy<Assembly>> _loadedAssemblies =
            new ConcurrentDictionary<string, Lazy<Assembly>>(StringComparer.OrdinalIgnoreCase);
        
        #endregion

        #region "Constructor"

        private IPlatoFileSystem _fileSystem;
        //private ILogger _logger;

        public ModuleLoaderService(
            IPlatoFileSystem fileSystem         
            )
        {
            _fileSystem = fileSystem;
            //_logger = logger;
        }

        #endregion

        #region "Implementation"
      
        public List<Assembly> LoadModule(ModuleDescriptor descriptor)
        {

            return LoadAssembliesInFolder(descriptor.Location, new List<Assembly>());

            //var assemblies = LoadAssembliesInFolder(
            //    descriptor.BinLocation)
            //if (!IsAmbientExtension(descriptor))            
            //    return Assembly.Load(new AssemblyName(descriptor.ID));

            //return null;
        }

        #endregion

        #region "Private Methods"

        private List<Assembly> LoadAssembliesInFolder(
            string path,
            List<Assembly> localList
            )
        {

            if (string.IsNullOrEmpty(path))
                return localList;

            var folder = _fileSystem.GetDirectoryInfo(path);
            
            foreach (var file in folder.GetFiles())
            {
                if (file.Extension.Contains(".dll"))
                {
                    //string fullPath = _fileSystem.Combine(folder.FullName, );
                    Assembly assembly = LoadFromAssemblyPath(file.FullName);
                    if (assembly != null)
                    {
                        localList.Add(assembly);
                    }
                }
            }

            string[] subFolders = Directory.GetDirectories(path);

            // recursive lookup
            if (folder != null)
            {
                for (int i = 0; i <= subFolders.Length - 1; i++)                
                    LoadAssembliesInFolder(subFolders.GetValue(i).ToString(), localList);                
            }
            
            return localList;

        }
                
        private Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            return _loadedAssemblies.GetOrAdd(Path.GetFileNameWithoutExtension(assemblyPath),
                new Lazy<Assembly>(() =>
                {
                    return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                })).Value;
        }


        private bool IsAmbientExtension(ModuleDescriptor descriptor)
        {
            return IsAmbientAssembly(descriptor.ID);
        }

        private static HashSet<string> ApplicationAssemblyNames => 
            _applicationAssemblyNames.Value;
        private static readonly Lazy<HashSet<string>> _applicationAssemblyNames =
            new Lazy<HashSet<string>>(GetApplicationAssemblyNames);

        private bool IsAmbientAssembly(string assemblyName)
        {
            return ApplicationAssemblyNames.Contains(assemblyName);
        }

        private static HashSet<string> GetApplicationAssemblyNames()
        {
            return new HashSet<string>(DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.RuntimeAssemblyGroups)
                .SelectMany(assetGroup => assetGroup.AssetPaths)
                .Select(path => Path.GetFileNameWithoutExtension(path)),
                StringComparer.OrdinalIgnoreCase);
        }
        

        private bool IsAssemblyLoaded(string assemblyName)
        {
            return _loadedAssemblies.ContainsKey(assemblyName);
        }

        #endregion

    }
}
