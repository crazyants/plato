using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Internal.Modules.Expanders
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {
        
        public ModuleViewLocationExpander()
        {
        }
        
        public void PopulateValues(ViewLocationExpanderContext context)
        {   
        }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            var result = new List<string>
            {
                "Modules/" + context.AreaName + "/Views/{1}/{0}" +  RazorViewEngine.ViewExtension,
                "Modules/" + context.AreaName + "/Views/Shared/{0}" + RazorViewEngine.ViewExtension
            };
            result.AddRange(viewLocations);
            return result;
        }

    }
}
