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
        private ILogger _logger;

        public ModuleLoader(
            IPlatoFileSystem fileSystem,
            ILogger<ModuleLoader> logger
        )
        {
            _fileSystem = fileSystem;
            _logger = logger;

            // ensure core assemblies are not loaded
            _loadedAssemblies.TryAdd("Plato.Abstractions", null);
            _loadedAssemblies.TryAdd("Plato.Cache", null);
            _loadedAssemblies.TryAdd("Plato.FileSystem", null);
            _loadedAssemblies.TryAdd("Plato.Data", null);
            _loadedAssemblies.TryAdd("Plato.Hosting", null);
            _loadedAssemblies.TryAdd("Plato.Hosting.Web", null);
            _loadedAssemblies.TryAdd("Plato.Layout", null);
            _loadedAssemblies.TryAdd("Plato.Localization", null);
            _loadedAssemblies.TryAdd("Plato.Modules", null);
            _loadedAssemblies.TryAdd("Plato.Modules.Abstractions", null);
            _loadedAssemblies.TryAdd("Plato.Models", null);
            _loadedAssemblies.TryAdd("Plato.Services", null);
            _loadedAssemblies.TryAdd("Plato.Shell", null);
            _loadedAssemblies.TryAdd("Plato.Shell.Abstractions", null);
            _loadedAssemblies.TryAdd("Plato.Repositories", null);
            _loadedAssemblies.TryAdd("Plato.Yaml", null);
            _loadedAssemblies.TryAdd("Plato.Stores", null);
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
            return _loadedAssemblies.GetOrAdd(Path.GetFileNameWithoutExtension(assemblyPath),
                    new Lazy<Assembly>(() => { return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath); }))
                .Value;
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