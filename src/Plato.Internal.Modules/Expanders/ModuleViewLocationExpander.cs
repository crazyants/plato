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


                // Ensure we first look for views in our current area
                "Modules/" + context.AreaName + "/Views/{1}/{0}" +  RazorViewEngine.ViewExtension,
                "Modules/" + context.AreaName + "/Views/Shared/{0}" + RazorViewEngine.ViewExtension,

                // Else allows us to load views from other modules
                "Modules/" + _id + "/Views/{1}/{0}" +  RazorViewEngine.ViewExtension,
                "Modules/" + _id + "/Views/Shared/{0}" + RazorViewEngine.ViewExtension

            };

            result.AddRange(viewLocations);
            return result;
        }

    }
}
