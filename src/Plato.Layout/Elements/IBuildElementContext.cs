using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Placement;

namespace Plato.Layout.Elements
{
    public interface IBuildElementContext
    {

        IElement Shape { get; }
        IElementFactory ElementFactory { get; }
        dynamic New { get; }
        IElement Layout { get; set; }
        string GroupId { get; }
        FindPlacementDelegate FindPlacement { get; set; }



    }
}
