using System;
using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.Views
{
    
    public interface IView
    {
        IEmbeddedView EmbeddedView { get; set; }

        string ViewName { get; set; }

        object Model { get; set; }
        
    }

    public class View : IView
    {

        public IEmbeddedView EmbeddedView { get; set; }
        
        public string ViewName { get; set; }
        
        public object Model { get; set; }
        
        public View(IEmbeddedView view)
        {
            this.ViewName = view.GetType().Name;
            this.EmbeddedView = view;
        }
        
        public View(string viewName, object model = null)
        {
            this.ViewName = viewName;
            this.Model = model;
        }
        
    }

}
