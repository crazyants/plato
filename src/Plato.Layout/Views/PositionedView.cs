using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plato.Layout.Views
{

    public interface IPositionedView : IView
    {

        ViewPosition Position { get;  }

        IPositionedView Zone(string zone);
        
        IPositionedView Order(int order);

    }

    public class PositionedView : View, IPositionedView
    {

        readonly string[] _supportedZoneNames = new string[]
        {
            "header",
            "meta",
            "content",
            "sidebar",
            "footer"
        };

        private string _zone;
        private int _order;
        
        public PositionedView(string viewName, object model) : 
            base(viewName, model)
        {
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
            if (!_supportedZoneNames.Contains(zone.ToLower()))
            {
                throw new Exception(
                    $"The zone name '{zone}' is not supported. Supported zones include {String.Join(",", _supportedZoneNames)}. Please update the zone naame.");
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
