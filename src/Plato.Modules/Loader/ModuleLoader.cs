using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Plato.FileSystem;
using Plato.Modules.Abstractions;

namespace Plato.Modules
{
    public class ModuleLoader : IModuleLoader
    {
        #region "Implementation"

        public List<Assembly> LoadModule(IModuleDescriptor descriptor)
        {
            return LoadAssembliesInFolder(descriptor.VirtualPathToBin, new List<Assembly>());
        }

        #endregion

        #region "Private Variables"

        private static readonly ConcurrentDictionary<string, Lazy<Assembly>> _loadedAssemblies =
            new ConcurrentDictionary<string, Lazy<Assembly>>(StringComparer.OrdinalIgnoreCase);

        private static HashSet<string> ApplicationAssemblyNames =>
            _applicationAssemblyNames.Value;

        private static readonly Lazy<HashSet<string>> _applicationAssemblyNames =
            new Lazy<HashSet<string>>(GetApplicationAssemblyNames);

        private const string AssemblyExtension = ".dll";

        #endregion

        #region "Constructor"

        private readonly IPlatoFileSystem _fileSystem;
        private readonly ILogger _logger;

        public ModuleLoader(
            IPlatoFileSystem fileSystem,
            ILogger<ModuleLoader> logger
        )
        {
            _fileSystem = fileSystem;
            _logger = logger;

            // ensure referenced assemblies are not loaded
            foreach (var name in ApplicationAssemblyNames)
                _loadedAssemblies.TryAdd(name, null);
       
        }

        #endregion

        #region "Private Methods"

        private List<Assembly> LoadAssembliesInFolder(
            string path, List<Assembly> localList)
        {
            if (string.IsNullOrEmpty(path))
                return localList;

            // no bin folder within module
            if (!_fileSystem.DirectoryExists(path))
                return localList;

            var folder = _fileSystem.GetDirectoryInfo(path);
            foreach (var file in folder.GetFiles())
                if ((file.Extension != null) && (file.Extension.ToLower() == AssemblyExtension))
                    if (!IsAssemblyLoaded(Path.GetFileNameWithoutExtension(file.FullName)))
                    {
                        var assembly = LoadFromAssemblyPath(file.FullName);
                        if (assembly != null)
                            localList.Add(assembly);
                    }

            // recursive lookup
            var subFolders = Directory.GetDirectories(path);
            for (var i = 0; i <= subFolders.Length - 1; i++)
                LoadAssembliesInFolder(subFolders.GetValue(i).ToString(), localList);

            return localList;
        }

        private Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            try
            {
                return _loadedAssemblies.GetOrAdd(
                   Path.GetFileNameWithoutExtension(assemblyPath),
                   new Lazy<Assembly>(() =>
                   {
                       return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                   }))
               .Value;
            }
            catch (Exception e)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Module loader failed to load assemby - {0}", e.Message);
                }
            }
            return null;

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