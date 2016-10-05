using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Mvc
{
    public interface IShapeTemplateViewEngine
    {
        IEnumerable<string> TemplateFileExtensions { get; }
    }
}
