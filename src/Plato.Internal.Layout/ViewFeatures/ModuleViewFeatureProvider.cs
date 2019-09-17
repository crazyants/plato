using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.ViewFeatures
{

    public class ModuleViewFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {

        private readonly IHostingEnvironment _hostingEnvironment;      
        private readonly IModuleManager _moduleManager;

        private ApplicationPartManager _applicationPartManager;
        private IEnumerable<IApplicationFeatureProvider<ViewsFeature>> _featureProviders;

        private readonly IServiceProvider _services;

        public ModuleViewFeatureProvider(IServiceProvider services)
        {
            _services = services;
            _hostingEnvironment = services.GetRequiredService<IHostingEnvironment>();            
            _moduleManager = services.GetRequiredService<IModuleManager>();            

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
                foreach (var module in _moduleManager.LoadModulesAsync().Result)
                {
                    // Does the module have a precompiled views asdembly?
                    if (module.ViewsAssembly != null)
                    {

                        var applicationPart = new ApplicationPart[]
                        {
                            new CompiledRazorAssemblyPart(module.ViewsAssembly)
                        };
                   
                        foreach (var provider in mvcFeatureProviders)
                        {
                            provider.PopulateFeature(applicationPart, moduleFeature);
                        }

                        // Razor views are precompiled in the context of their modules, but at runtime
                        // their paths need to be relative to the virtual "Areas/{ModuleId}" folders.
                        // Note: For the app's module this folder is "Areas/{env.ApplicationName}".
                        foreach (var descriptor in moduleFeature.ViewDescriptors)
                        {
                            descriptor.RelativePath = "~/Modules/" + module.Descriptor.Id + descriptor.RelativePath;
                            feature.ViewDescriptors.Add(descriptor);
                        }

                        moduleFeature.ViewDescriptors.Clear();

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
