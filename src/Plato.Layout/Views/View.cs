
namespace Plato.Layout.Views
{
    
    public interface IView
    {
        string ViewName { get; set; }

        object Model { get; set; }
        
    }

    public class View : IView
    {
        public string ViewName { get; set; }
        
        public object Model { get; set; }
        
        public View(string viewName, object model)
        {
            this.ViewName = viewName;
            this.Model = model;
        }

    }
}
