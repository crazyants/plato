using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Modules
{

    public class ModuleEmbeddedStaticFileProvider : IFileProvider
    {

        private readonly IOptions<ModuleOptions> _moduleOptions;
        private readonly IModuleManager _moduleManager;

        private IList<IModuleEntry> _modules;        
        private string _root;
        private string _moduleRoot;
        private IList<string> _staticFolders = new List<string>
            {
                "/Sites",
                "/Themes",
                "/Locales"
            };
        
        public ModuleEmbeddedStaticFileProvider(IHostingEnvironment eng, IServiceProvider services)
        {
            _moduleOptions = services.GetRequiredService<IOptions<ModuleOptions>>();
            _moduleManager = services.GetRequiredService<IModuleManager>();            
            _root = eng.ContentRootPath + "\\";
            _moduleRoot = _moduleOptions.Value.VirtualPathToModulesFolder;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (subpath == null)
            {
                return new NotFoundFileInfo(subpath);
            }
                    

            if (_modules == null)
            {
                _modules = _moduleManager.LoadModulesAsync()
                    .GetAwaiter()
                    .GetResult()
                    .ToList();
            }

            var path = NormalizePath(subpath);
            var index = path.IndexOf('/');

            // "/**/*.*".
            if (index != -1)
            {
                
                // Resolve the module id.
                var module = path.Substring(0, index);

                // Check if it is a module request
                if (_modules.Any(m => m.Descriptor.Id.Equals(module, StringComparison.OrdinalIgnoreCase)))
                {

                    // Resolve the embedded file subpath: "Content/**/*.*"
                    var fileSubPath = path.Substring(index + 1).Replace("/", "\\");

                    // We only serve static files from the modules "Content" directory
                    if (fileSubPath.StartsWith("Content", StringComparison.OrdinalIgnoreCase))
                    {
                       
                        var contentPath = Path.Combine(                                
                                _moduleRoot,
                                module,
                                fileSubPath);

                        //var filePath = _root + "\\" + ileSubPath.Replace("/", "\\")f;
                        return new PhysicalFileInfo(new FileInfo(_root + contentPath));
                    }
              
                }
                else
                {


                    var fileSubPath = subpath.Replace("/", "\\");
                                      
                    var isCustomStaticFolder = false;
                    foreach (var staticFolder in _staticFolders)
                    {
                        isCustomStaticFolder = subpath.StartsWith(staticFolder, StringComparison.OrdinalIgnoreCase);
                        if (isCustomStaticFolder)
                        {
                            break;
                        }
                    }
                                       
                    if (isCustomStaticFolder)
                    {                
                        return new PhysicalFileInfo(new FileInfo(_root + fileSubPath));
                    }
                    else
                    {
                        var filePath = _root + "wwwroot" + fileSubPath;
                        return new PhysicalFileInfo(new FileInfo(filePath));
                    }                       
                  
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
    }
}
