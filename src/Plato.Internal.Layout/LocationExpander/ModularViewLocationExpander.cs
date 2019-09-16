using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.FileProviders;
using Plato.Internal.Models.Modules;
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

            var result = new List<string>();            
            foreach (var module in _moduleManager.LoadModulesAsync().Result)
            {

                result.Add($"~/Modules/{module.Descriptor.Id}/Views/{{1}}/{{0}}{RazorViewEngine.ViewExtension}");
                result.Add($"~/Modules/{module.Descriptor.Id}/Views/Shared/{{0}}{RazorViewEngine.ViewExtension}");

                //var result = new List<string>
                //{
                //    // Else allows us to load views from other modules                
                //    ,
                    
                //};

            }

            result.AddRange(viewLocations);
            return result;
        }

    }
         
    
}
