

namespace Plato.Layout.Views
{
    
    public interface IGenericView
    {
        string ViewName { get; set; }

        object Model { get; set; }
        
        string[] AlternateViews { get; set; }

    }

    public class GenericView : IGenericView
    {
        public string ViewName { get; set; }
        
        public object Model { get; set; }

        public string[] AlternateViews { get; set; }

        public GenericView(string viewName, object model)
        {
            this.ViewName = viewName;
            this.Model = model;
        }

    }
}
