using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.ViewFeatures
{

    public class ModuleViewFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {
           
        private ApplicationPartManager _applicationPartManager;
        private IEnumerable<IApplicationFeatureProvider<ViewsFeature>> _featureProviders;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModuleManager _moduleManager;
        private readonly IServiceProvider _services;

        // TODO: Refactor to obtain from IOptions
        private string _moduleRoot = "/Modules/";

        public ModuleViewFeatureProvider(IServiceProvider services)
        {            
            _hostingEnvironment = services.GetRequiredService<IHostingEnvironment>();            
            _moduleManager = services.GetRequiredService<IModuleManager>();
            _services = services;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {

            // The scope is null when this code is called through a 'ChangeToken' callback, e.g to recompile razor pages.
            // So, here we resolve and cache tenant level singletons, application singletons can be resolved in the ctor.

            if (_services != null && _featureProviders == null)
            {
                lock (this)
                {
                    if (_featureProviders == null)
                    {
                        _applicationPartManager = _services.GetRequiredService<ApplicationPartManager>();
                        _featureProviders = _services.GetServices<IApplicationFeatureProvider<ViewsFeature>>();
                    }
                }
            }
            
            // Module compiled views are not served while in dev.
            if (!_hostingEnvironment.IsDevelopment())
            {

                // Retrieve mvc views feature providers but not this one.
                var mvcFeatureProviders = _applicationPartManager.FeatureProviders
                    .OfType<IApplicationFeatureProvider<ViewsFeature>>()
                    .Where(p => p.GetType() != typeof(ModuleViewFeatureProvider));

                var moduleFeature = new ViewsFeature();
                var modules = _moduleManager.LoadModulesAsync().Result;

                foreach (var module in modules)
                {

                    var precompiledAssemblyPath = Path.Combine(Path.GetDirectoryName(module.Assembly.Location),
                      module.Assembly.GetName().Name + ".Views.dll");

                    if (File.Exists(precompiledAssemblyPath))
                    {
                        try
                        {

                            var assembly = Assembly.LoadFile(precompiledAssemblyPath);

                            var applicationPart = new ApplicationPart[]
                           {
                                    new CompiledRazorAssemblyPart(assembly)
                           };

                            foreach (var provider in mvcFeatureProviders)
                            {
                                provider.PopulateFeature(applicationPart, moduleFeature);
                            }

                            // Razor views are precompiled in the context of their modules, but at runtime
                            // their paths need to be relative to the application root.                        
                            foreach (var descriptor in moduleFeature.ViewDescriptors)
                            {
                                descriptor.RelativePath = _moduleRoot + module.Descriptor.Id + descriptor.RelativePath;
                                feature.ViewDescriptors.Add(descriptor);
                            }

                            moduleFeature.ViewDescriptors.Clear();

                        }
                        catch (FileLoadException)
                        {
                            // Don't throw if assembly cannot be loaded.
                            // This can happen if the file is not a managed assembly.
                        }
                        
                    }

                }

            }

            // Apply views feature providers registered at the tenant level.
            foreach (var provider in _featureProviders)
            {
                provider.PopulateFeature(parts, feature);
            }

        }

    }

}
