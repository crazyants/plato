using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;

namespace Plato.Layout.Placement
{
    public delegate PlacementInfo FindPlacementDelegate(
        IElement element, 
        string differentiator, 
        string displayType);
}
