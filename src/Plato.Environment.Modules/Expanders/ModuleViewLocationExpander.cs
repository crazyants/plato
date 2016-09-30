using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Environment.Modules
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {


        private string _moduleId;

        public ModuleViewLocationExpander(string moduleId)
        {
            _moduleId = moduleId;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
            
        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,        
            IEnumerable<string> viewLocations)
        {

            var result = new List<string>();
            result.Add("/Modules/" + _moduleId  + "/Views/Shared/{0}.cshtml");
            result.AddRange(viewLocations);   
            return result;

        }
    }

}
