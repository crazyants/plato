using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Modules
{
    public class ModuleViewLocationExpander : IViewLocationExpander
    {

        private string _moduleId;
        private const string _moduleKey = "module";
        private const string _controllerKey = "controller";

        public ModuleViewLocationExpander(string moduleId)
        {
            _moduleId = moduleId;

            //var expander = new ExtensionLocationExpander(
            // DefaultExtensionTypes.Module,
            // new[] { virtualPath },
            // "Module.txt"
            // );

            //options.ExtensionLocationExpanders.Add(expander);

        }


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //var controller = context.ActionContext.ActionDescriptor.DisplayName;
            //var moduleName = controller.Split('.')[2];
            //if (moduleName != "WebHost")
            //{
            //    context.Values[_controllerKey] = controller;
            //    context.Values[_moduleKey] = moduleName;
            //}
        }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {

            var result = new List<string>();

            result.Add("/Modules/{2}/Views/{1}/{0}.cshtml");
            result.Add("/Modules/{2}/Views/Shared/{0}.cshtml");
            
            result.Add("Modules/" + _moduleId + "/Views/{1}/{0}.cshtml");
            result.Add("Modules/" + _moduleId + "/Views/Shared/{0}.cshtml");


            //result.Add("{2}/Modules/" + _moduleId + "/Views/{1}/{0}.cshtml");
            //result.Add("{2}/Modules/" + _moduleId + "/Views/Shared/{0}.cshtml");

            //result.Add("{2}/Modules/Plato.Login/Views/{1}/{0}.cshtml");
            //result.Add("{2}/Modules/Plato.Login/Views/Shared/{0}.cshtml");

            result.AddRange(viewLocations);

            return result;

            //if (context.Values.ContainsKey(_moduleKey))
            //{
            //    var module = context.Values[_moduleKey];
            //    if (!string.IsNullOrWhiteSpace(module))
            //    {
            //var moduleViewLocations = new string[]
            //        {
            //           "/Modules/" + _moduleId + "/Views/{1}/{0}.cshtml",
            //           "/Modules/" + _moduleId + "/Views/Shared/{0}.cshtml",
            //           "/Modules/{2}/Views/{1}/{0}.cshtml",
            //           "/Modules/{2}/Views/Shared/{0}.cshtml"
            //        };

            //        viewLocations = moduleViewLocations.Concat(viewLocations);
            ////    }
            ////}
            return viewLocations;
        }

    }
}
