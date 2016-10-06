using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Plato.FileSystem;
using Microsoft.AspNetCore.Mvc;
using Plato.Layout.Mvc;

namespace Plato.Layout.Elements.TemplateStratagy
{
    public class TemplateBindingStrategy
    {

        private readonly IEnumerable<IElementTemplateHarvester> _harvesters;
        private readonly IEnumerable<IShapeTemplateViewEngine> _shapeTemplateViewEngines;
        private readonly IOptions<MvcViewOptions> _viewEngine;
        private readonly IPlatoFileSystem _fileSystem;
    
            
    }
}
