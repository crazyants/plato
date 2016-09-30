using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Environment.Modules
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {
            
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
            
        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,        
            IEnumerable<string> viewLocations)
        {
            var result = new List<string>();
            result.Add("/Modules/{2}/Views/Shared/Components/{1}/{0}.cshtml");

            result.AddRange(viewLocations);
   
            return result;
        }
    }

}
