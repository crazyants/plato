using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.Handlers
{
    public class BuildEditorContext : BuildShapeContext
    {

        public BuildEditorContext(
            IElement shape, 
            string groupId, 
            IElementFactory shapeFactory, 
            IElement layout, 
            IUpdateModel updater)
        : base(shape, groupId, shapeFactory, layout, updater)
        {
        }

    }
}
