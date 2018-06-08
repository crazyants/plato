

using System.Threading.Tasks;
using Plato.Layout.Drivers;

namespace Plato.Layout.Views
{
    
    public interface IGenericView
    {
        string ViewName { get; set; }

        object Model { get; set; }
        
        string[] AlternateViews { get; set; }

        Task ApplyAsync(ProviderDisplayContext context);
        Task ApplyAsync(ProviderEditContext context);
    }

    public class GenericView : IGenericView
    {
        public string ViewName { get; set; }
        
        public object Model { get; set; }

        public string[] AlternateViews { get; set; }
        public Task ApplyAsync(ProviderDisplayContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ApplyAsync(ProviderEditContext context)
        {
            throw new System.NotImplementedException();
        }

        public GenericView(string viewName, object model)
        {
            this.ViewName = viewName;
            this.Model = model;
        }

    }
}
