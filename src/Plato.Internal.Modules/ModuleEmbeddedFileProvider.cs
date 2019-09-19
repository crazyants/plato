using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Modules.FileProviders;
using Plato.Internal.Modules.Models;

namespace Plato.Internal.Modules
{
    public class ModuleEmbeddedFileProvider : IFileProvider
    {
            
        private readonly IRazorViewEngineFileProviderAccessor _fileProviderAccessor;
        private readonly IOptions<ModuleOptions> _moduleOptions;
        private readonly IModuleManager _moduleManager;

        private IList<IModuleEntry> _modules;
        private string _baseNamespace = "Plato";        
        private string _modulesFolder;
        private string _modulesRoot;
        private string _root;

        public ModuleEmbeddedFileProvider(IServiceProvider services)
        {

            var env = services.GetRequiredService<IHostingEnvironment>();
            _moduleOptions = services.GetRequiredService<IOptions<ModuleOptions>>();
            _moduleManager = services.GetRequiredService<IModuleManager>();
            _fileProviderAccessor = services.GetRequiredService<IRazorViewEngineFileProviderAccessor>();
            
            _modulesFolder = _moduleOptions.Value.VirtualPathToModulesFolder;
            _modulesRoot = _moduleOptions.Value.VirtualPathToModulesFolder + "/";
            _root = env.ContentRootPath;

        }
          
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null)
            {
                return NotFoundDirectoryContents.Singleton;
            }
            
            if (_modules == null)
            {
                _modules = _moduleManager.LoadModulesAsync()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();
            }
         
            var folder = NormalizePath(subpath);
            var entries = new List<IFileInfo>();
            
            // Under the root.
            if (folder == "")
            {
                // Add the virtual folder "Areas" containing all modules.
                entries.Add(new EmbeddedDirectoryInfo(_modulesFolder));
            }
            // Under "Modules".
            else if (folder == _modulesFolder)
            {
                // Add virtual folders for all modules by using their assembly names (module ids).
                entries.AddRange(
                    _modules.Select(m => new EmbeddedDirectoryInfo(m.Descriptor.Id)));
            }
            // Under "Modules/{ModuleId}" or "Modules/{ModuleId}/**".
            else if (folder.StartsWith(_modulesRoot, StringComparison.Ordinal))
            {

                // Skip "Modules/" from the folder path.
                var path = folder.Substring(_modulesRoot.Length);
                var index = path.IndexOf('/');

                // Resolve the module id and get all its asset paths.
                var name = index == -1 ? path : path.Substring(0, index);

                var ph = _root + _modulesFolder + "\\" + name;
                var paths = new List<string>();             
                foreach (var file  in _fileProviderAccessor.FileProvider.GetDirectoryContents(path))
                {
                    paths.Add(path + name + file.Name);
                    paths.Add(folder + file.Name);
                }

                // Resolve all files and folders directly under this given folder.
                NormalizedPaths.ResolveFolderContents(folder, paths, out var files, out var folders);

                // And add them to the directory contents.
                entries.AddRange(files.Select(p => GetFileInfo(p)));
                entries.AddRange(folders.Select(n => new EmbeddedDirectoryInfo(n)));
            }

            return new EmbeddedDirectoryContents(entries);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }

            var path = NormalizePath(subpath);
            var modulesFolder = _moduleOptions.Value.VirtualPathToModulesFolder;
            var modulesRoot = _moduleOptions.Value.VirtualPathToModulesFolder + "/";

            // "Modules/**/*.*".
            if (path.StartsWith(modulesRoot, StringComparison.Ordinal))
            {
                // Skip the "Modules/" root.
                path = path.Substring(modulesRoot.Length);
                var index = path.IndexOf('/');

                // "{ModuleId}/**/*.*".
                if (index != -1)
                {
                    // Resolve the module id.
                    var moduleId = path.Substring(0, index);
                    var module = _modules.FirstOrDefault(m => m.Descriptor.Id.Equals(moduleId, StringComparison.OrdinalIgnoreCase));

                    // Skip the module id to resolve the subpath.
                    var fileSubPath = path.Substring(index + 1);

                    //// If it is the app's module.
                    //if (moduleId == Application.Name)
                    //{
                    //    // Serve the file from the physical application root folder.
                    //    return new PhysicalFileInfo(new FileInfo(Application.Root + fileSubPath));
                    //}

                    // Get the embedded file info from the module assembly.
                    return GetFileInfos(fileSubPath, module);
                }
            }

            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Trim('/').Replace("//", "/");
        }


        private readonly IDictionary<string, IFileInfo> _fileInfos = new Dictionary<string, IFileInfo>();

        public IFileInfo GetFileInfos(string subpath, IModuleEntry module)
        {
            if (!_fileInfos.TryGetValue(subpath, out var fileInfo))
            {
                //if (!assetPaths.Contains(_root + subpath, StringComparer.Ordinal))
                //{
                //    return new NotFoundFileInfo(subpath);
                //}

                lock (_fileInfos)
                {
                    if (!_fileInfos.TryGetValue(subpath, out fileInfo))
                    {
                        var resourcePath = _baseNamespace + subpath.Replace('/', '>');
                        var fileName = Path.GetFileName(subpath);

                        if (module.Assembly.GetManifestResourceInfo(resourcePath) == null)
                        {
                            return new NotFoundFileInfo(fileName);
                        }

                        _fileInfos[subpath] = fileInfo = new EmbeddedResourceFileInfo(
                            module.Assembly, resourcePath, fileName, DateTime.Now);
                    }
                }
            }

            return fileInfo;
        }

    }

}
