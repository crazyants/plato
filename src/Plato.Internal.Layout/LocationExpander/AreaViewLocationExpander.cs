using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Internal.Layout.LocationExpander
{
    public class AreaViewLocationExpander : IViewLocationExpanderProvider
    {

        public int Priority => 6;

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {

            var result = new List<string>
            {                
                $"~/Modules/{{2}}/Views/{{1}}/{{0}}{RazorViewEngine.ViewExtension}",
                $"~/Modules/{{2}}/Views/Shared/{{0}}{RazorViewEngine.ViewExtension}"
            };

            result.AddRange(viewLocations);
            return result;
        }

    }

}
