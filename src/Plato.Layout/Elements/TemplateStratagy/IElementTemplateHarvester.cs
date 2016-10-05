using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements.TemplateStratagy
{
    public interface IElementTemplateHarvester
    {
        IEnumerable<string> SubPaths();
        IEnumerable<HarvestShapeHit> HarvestShape(HarvestShapeInfo info);
    }
}
