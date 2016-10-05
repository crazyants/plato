using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Mvc.Razor
{
    public class RazorShapeTemplateViewEngine : IShapeTemplateViewEngine
    {
        public IEnumerable<string> TemplateFileExtensions
        {
            get
            {
                return new[] { "cshtml" };
            }
        }
    }
}
