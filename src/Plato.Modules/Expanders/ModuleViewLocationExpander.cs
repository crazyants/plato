using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Modules.Expanders
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {

        private readonly string _moduleId;
        private const string _moduleKey = "module";
        
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
            var result = new List<string>
            {
                "Modules/" + _moduleId + "/Views/{1}/{0}.cshtml",
                "Modules/" + _moduleId + "/Views/Shared/{0}.cshtml"
            };
            result.AddRange(viewLocations);
            return result;
        }

    }
}
