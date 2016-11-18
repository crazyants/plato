using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Plato.Hosting.Web.Expanders
{
    public class ThemeViewLocationExpander : IViewLocationExpander
    {
        readonly string _theme;

        public ThemeViewLocationExpander(string theme)
        {
            _theme = theme;
        }

        /// <inheritdoc />
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            var result = new List<string>();

            result.Add("/Themes/" + _theme  + "/{1}/{0}.cshtml");
            result.Add("/Themes/" + _theme + "/Shared/{0}.cshtml");

            result.AddRange(viewLocations);

            return result;
        }
    }

}
