using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Plato.Internal.FileSystem.Abstractions;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Modules.Loader
{
    public class ModuleLoader : IModuleLoader
    {
        #region "Implementation"

        /// <inheritdoc />
        public async Task<List<Assembly>> LoadModuleAsync(IModuleDescriptor descriptor)
        {
            if (descriptor.Id.Equals("Plato.Core", StringComparison.OrdinalIgnoreCase))
            {
                var test = "test";
            }
            return await LoadAssembliesInFolder(descriptor.VirtualPathToBin.Replace("/","\\"), new List<Assembly>());
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
        private readonly ILogger<ModuleLoader> _logger;

        public ModuleLoader(
            IPlatoFileSystem fileSystem,
            ILogger<ModuleLoader> logger)
        {
            _fileSystem = fileSystem;
            _logger = logger;

            // ensure referenced assemblies are not loaded
            foreach (var name in ApplicationAssemblyNames)
            {
                _loadedAssemblies.TryAdd(name, null);
            }
                

        }

        #endregion

        #region "Private Methods"

        private async Task<List<Assembly>> LoadAssembliesInFolder(
            string path, List<Assembly> localList)
        {
            if (string.IsNullOrEmpty(path))
                return localList;

            // no bin folder within module
            if (!_fileSystem.DirectoryExists(path))
                return localList;
            
            var folder = _fileSystem.GetDirectoryInfo(path);
            foreach (var file in folder.GetFiles())
            {
                if (file.Extension.ToLower() == AssemblyExtension)
                {
                    if (!IsAssemblyLoaded(Path.GetFileNameWithoutExtension(file.FullName)))
                    {
                        var assembly = LoadFromAssemblyPath(file.FullName);
                        if (assembly != null)
                            localList.Add(assembly);
                    }
               
                }
                
            }

            // Recursive lookup
            var subFolders = Directory.GetDirectories(path);
            for (var i = 0; i <= subFolders.Length - 1; i++)
            {
                await LoadAssembliesInFolder(subFolders.GetValue(i).ToString(), localList);
            }
                
            return localList;
        }

        private Assembly LoadFromAssemblyPath(string assemblyPath)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);

            if (String.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentNullException(nameof(assemblyName));
            }

            return _loadedAssemblies.GetOrAdd(assemblyName,
                    new Lazy<Assembly>(() => AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath)))
                .Value;
        }

        private static HashSet<string> GetApplicationAssemblyNames()
        {
            return new HashSet<string>(
                    DependencyContext.Default.RuntimeLibraries
                    .SelectMany(library => library.RuntimeAssemblyGroups)
                    .SelectMany(assetGroup => assetGroup.AssetPaths)
                    .Select(Path.GetFileNameWithoutExtension),
                StringComparer.OrdinalIgnoreCase);
        }
        
        private bool IsAssemblyLoaded(string assemblyName)
        {
            return _loadedAssemblies.ContainsKey(assemblyName);
        }

        #endregion

    }

}