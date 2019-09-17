using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Layout.LocationExpander
{

    public class ModularViewLocationExpander : IViewLocationExpanderProvider
    {
        private readonly IModuleManager _moduleManager;

        public ModularViewLocationExpander(IModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public int Priority =>  5;

        public void PopulateValues(ViewLocationExpanderContext context)
        {   
        }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {

            // TODO: Add IMemoryCache
            var result = new List<string>();            
            foreach (var module in _moduleManager.LoadModulesAsync().Result)
            {
                result.Add($"~/Modules/{module.Descriptor.Id}/Views/{{1}}/{{0}}{RazorViewEngine.ViewExtension}");
                result.Add($"~/Modules/{module.Descriptor.Id}/Views/Shared/{{0}}{RazorViewEngine.ViewExtension}");
            }

            result.AddRange(viewLocations);
            return result;
        }

    }
         
    
}
