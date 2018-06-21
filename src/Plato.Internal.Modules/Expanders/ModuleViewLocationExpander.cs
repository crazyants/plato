using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Internal.Modules.Expanders
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {

        private readonly string _id;



        public ModuleViewLocationExpander(string id)
        {
            _id = id;
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
                "Modules/" + _id + "/Views/{1}/{0}" +  RazorViewEngine.ViewExtension,
                "Modules/" + _id + "/Views/Shared/{0}" + RazorViewEngine.ViewExtension
            };
            result.AddRange(viewLocations);
            return result;
        }

    }
}
