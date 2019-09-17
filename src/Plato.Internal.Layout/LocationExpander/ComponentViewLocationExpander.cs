using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.FileProviders;
using Plato.Internal.Models.Modules;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.LocationExpander
{
    public class ComponentViewLocationExpander : IViewLocationExpanderProvider
    {

        private static IList<IModuleEntry> _modulesWithComponentViews;

        private IModuleManager _moduleManager;

        public int Priority => 5;

        public ComponentViewLocationExpander(
            IRazorViewEngineFileProviderAccessor fileProviderAccessor,
            IModuleManager moduleManager)
        {

            _moduleManager = moduleManager;

            if (_modulesWithComponentViews == null)
            {

                foreach (var module in moduleManager.LoadModulesAsync().Result)
                {
                    var moduleComponentsViewFilePaths = fileProviderAccessor.FileProvider.GetViewFilePaths(
                              "Modules/" + module.Descriptor.Id + "/Views/Shared/Components",
                              new[] {
                                  RazorViewEngine.ViewExtension
                              },
                              viewsFolder: null,
                              inViewsFolder: true,
                              inDepth: true);

                    if (moduleComponentsViewFilePaths.Any())
                    {
                        if (_modulesWithComponentViews == null)
                        {
                            _modulesWithComponentViews = new List<IModuleEntry>();
                        }
                        _modulesWithComponentViews.Add(module);
                    }
                }
            }


        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
                                                               IEnumerable<string> viewLocations)
        {
            if (context.AreaName == null)
            {
                return viewLocations;
            }

            var result = new List<string>();

            if (context.ViewName.StartsWith("Components/", StringComparison.Ordinal))
            {

                var moduleComponentViewLocations = new List<string>();

                //if (!_memoryCache.TryGetValue(CacheKey, out IEnumerable<string> moduleComponentViewLocations))
                //{
                //var enabledIds = _extensionManager.GetFeatures().Where(f => _shellDescriptor
                //    .Features.Any(sf => sf.Id == f.Id)).Select(f => f.Extension.Id).Distinct().ToArray();

                //var enabledExtensions = _extensionManager.GetExtensions()
                //    .Where(e => enabledIds.Contains(e.Id)).ToArray();

                if (_modulesWithComponentViews != null)
                {

                    var sharedViewsPath = "/Views/Shared/{0}" + RazorViewEngine.ViewExtension;
                    moduleComponentViewLocations = _modulesWithComponentViews
                        //.Where(m => enabledExtensions.Any(e => e.Id == m.Id))
                        .Select(m => '/' + m.Descriptor.Id + sharedViewsPath)
                        .ToList();

                    //_memoryCache.Set(CacheKey, moduleComponentViewLocations);
                    //}

                    result.AddRange(moduleComponentViewLocations);
                }

            }

            result.AddRange(viewLocations);

            return result;
        }


    }

    public static class FileProviderExtensions
    {
        public static IEnumerable<string> GetViewFilePaths(this IFileProvider fileProvider,
            string subPath,
            string[] extensions,
            string viewsFolder = null,
            bool inViewsFolder = false,
            bool inDepth = true)
        {
            var contents = fileProvider.GetDirectoryContents(subPath);

            if (contents == null)
            {
                yield break;
            }

            if (!inViewsFolder && viewsFolder != null && inDepth)
            {
                var viewsFolderInfo = contents.FirstOrDefault(c => c.Name == viewsFolder && c.IsDirectory);

                if (viewsFolderInfo != null)
                {
                    foreach (var filePath in GetViewFilePaths(fileProvider, $"{subPath}/{viewsFolderInfo.Name}", extensions, viewsFolder, inViewsFolder: true))
                    {
                        yield return filePath;
                    }

                    yield break;
                }
            }

            foreach (var content in contents)
            {
                if (content.IsDirectory && inDepth)
                {
                    foreach (var filePath in GetViewFilePaths(fileProvider, $"{subPath}/{content.Name}", extensions, viewsFolder, inViewsFolder))
                    {
                        yield return filePath;
                    }
                }
                else if (inViewsFolder)
                {
                    if (extensions.Contains(Path.GetExtension(content.Name)))
                    {
                        yield return $"{subPath}/{content.Name}";
                    }
                }
            }
        }
    }

}
