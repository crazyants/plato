
using System;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Layout.CustomViews;

namespace Plato.Internal.Layout.Views
{
    
    public interface IView
    {
        IViewContent ViewContent { get; set; }

        string ViewName { get; set; }

        object Model { get; set; }
        
    }

    public class View : IView
    {

        public IViewContent ViewContent { get; set; }
        
        public string ViewName { get; set; }
        
        public object Model { get; set; }

        public View()
        {

        }

        public View(IViewContent content, object model = null)
        {
            this.ViewName = GetViewNameForContentType(content.GetType());
            this.ViewContent = content;
            this.Model = model;
        }
        
        public View(string viewName, object model = null)
        {
            this.ViewName = viewName;
            this.Model = model;
        }


        private string GetViewNameForContentType(Type type)
        {

            return type.ToString();
        }

    }

}
