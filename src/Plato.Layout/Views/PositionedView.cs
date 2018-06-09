using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Layout.Views
{

    public interface IPositionedView : IView
    {

        Position ViewPosition { get;  }

        IPositionedView Zone(string zone);
        
        IPositionedView Order(int order);

    }

    public class PositionedView : View, IPositionedView
    {

        private string _zone;
        private int _order;
        
        public PositionedView(string viewName, object model) : 
            base(viewName, model)
        {
        }
        
        public Position ViewPosition => new Position(_zone, _order);
        
        public IPositionedView Zone(string zone)
        {
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
