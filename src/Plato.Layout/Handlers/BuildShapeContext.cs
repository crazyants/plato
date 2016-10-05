using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;
using Plato.Layout.ModelBinding;
using Plato.Layout.Placement;

namespace Plato.Layout.Handlers
{
    public abstract class BuildShapeContext : IBuildShapeContext
    {
        protected BuildShapeContext(
            IElement shape, 
            string groupId,
            IElementFactory shapeFactory, 
            IElement layout,
            IUpdateModel updater)
        {
            Shape = shape;
            ElementFactory = shapeFactory;
            GroupId = groupId;
            Layout = layout;
            FindPlacement = FindDefaultPlacement;
            Updater = updater;
        }

        public IElement Shape { get; private set; }
        public IElementFactory ElementFactory { get; private set; }
        public dynamic New => ElementFactory;
        public IElement Layout { get; set; }
        public string GroupId { get; private set; }
        public FindPlacementDelegate FindPlacement { get; set; }
        public IUpdateModel Updater { get; }

        private static PlacementInfo FindDefaultPlacement(
            IElement element, 
            string differentiator,
            string displayType)
        {
            return null;
        }
    }

}
