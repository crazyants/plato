using System;
using System.Linq;
using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.Views
{

    public interface IPositionedView : IView
    {

        ViewPosition Position { get; }

        IPositionedView Zone(string zone);
        
        IPositionedView Order(int order);

    }

    public class PositionedView : View, IPositionedView
    {
     
        private string _zone;
        private int _order;
        
        public PositionedView(IEmbeddedView embeddedView) :
            base(embeddedView)
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }

        public PositionedView(string viewName, object model) : 
            base(viewName, model)
        {
            _zone = LayoutZones.ContentZoneName;
            _order = 1;
        }
        
        public ViewPosition Position => new ViewPosition(_zone, _order);
        
        public IPositionedView Zone(string zone)
        {

            // We already expect a zone
            if (String.IsNullOrEmpty(zone))
            {
                throw new Exception(
                    $"No znon has been specified for the view {this.ViewName}.");
            }
            
            // Is the zone supported?
            var supprtedZones = LayoutZones.SupportedZones;
            if (!supprtedZones.Contains(zone.ToLower()))
            {
                throw new Exception(
                    $"The zone name '{zone}' is not supported. Supported zones include {String.Join(", ", supprtedZones)}. Please update the zone name within your view provider and try again.");
            }

            _zone = zone;
            return this;
        }

        public IPositionedView Order(int order)
        {
            _order = order;
            return this;
        }

    }
}
