using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;


namespace Plato.Environment.Modules.Loader
{
    //public class CustomDirectoryModuleProvider : IAssemblyProvider
    //{
    //    private readonly IFileProvider _fileProvider;
    //    private readonly IAssemblyLoadContextAccessor _loadContextAccessor;
    //    private readonly IAssemblyLoaderContainer _assemblyLoaderContainer;

    //    public CustomDirectoryAssemblyProvider(
    //            IFileProvider fileProvider,
    //            IAssemblyLoadContextAccessor loadContextAccessor,
    //            IAssemblyLoaderContainer assemblyLoaderContainer)
    //    {
    //        _fileProvider = fileProvider;
    //        _loadContextAccessor = loadContextAccessor;
    //        _assemblyLoaderContainer = assemblyLoaderContainer;
    //    }

    //    public IEnumerable<Assembly> CandidateAssemblies
    //    {
    //        get
    //        {
    //            var content = _fileProvider.GetDirectoryContents("/App_Plugins");
    //            if (!content.Exists) yield break;
    //            foreach (var pluginDir in content.Where(x => x.IsDirectory))
    //            {
    //                var binDir = new DirectoryInfo(Path.Combine(pluginDir.PhysicalPath, "bin"));
    //                if (!binDir.Exists) continue;
    //                foreach (var assembly in GetAssembliesInFolder(binDir))
    //                {
    //                    yield return assembly;
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Returns assemblies loaded from /bin folders inside of App_Plugins
    //    /// </summary>
    //    /// <param name="binPath"></param>
    //    /// <returns></returns>
    //    private IEnumerable<Assembly> GetAssembliesInFolder(DirectoryInfo binPath)
    //    {
    //        // Use the default load context
    //        var loadContext = _loadContextAccessor.Default;

    //        // Add the loader to the container so that any call to Assembly.Load 
    //        // will call the load context back (if it's not already loaded)
    //        using (_assemblyLoaderContainer.AddLoader(
    //            new DirectoryLoader(binPath, loadContext)))
    //        {
    //            foreach (var fileSystemInfo in binPath.GetFileSystemInfos("*.dll"))
    //            {
    //                //// In theory you should be able to use Assembly.Load() here instead
    //                //var assembly1 = Assembly.Load(AssemblyName.GetAssemblyName(fileSystemInfo.FullName));
    //                var assembly2 = loadContext.Load(AssemblyName.GetAssemblyName(fileSystemInfo.FullName));
    //                yield return assembly2;
    //            }
    //        }
    //    }

    //    public interface IAssemblyLoadContextAccessor
    //    {
    //    }
    //}
}
