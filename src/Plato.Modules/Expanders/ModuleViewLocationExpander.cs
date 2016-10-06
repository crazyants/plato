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
        private const string _moduleKey = "module";

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
            var controller = context.ActionContext.ActionDescriptor.DisplayName;
            var moduleName = controller.Split('.')[2];
            if (moduleName != "WebHost")
            {
                context.Values[_moduleKey] = _moduleId;
            }
        }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {

            if (context.Values.ContainsKey(_moduleKey))
            {
                var module = context.Values[_moduleKey];
                if (!string.IsNullOrWhiteSpace(module))
                {
                    var moduleViewLocations = new string[]
                    {
                       "/Modules/" + module + "/Views/{1}/{0}.cshtml",
                       "/Modules/" + module + "/Views/Shared/{0}.cshtml"
                    };

                    viewLocations = moduleViewLocations.Concat(viewLocations);
                }
            }
            return viewLocations;
        }

    }
}
