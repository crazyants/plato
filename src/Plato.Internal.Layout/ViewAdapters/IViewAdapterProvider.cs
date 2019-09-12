using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdapters
{

    public interface IViewAdapterProvider
    {
        
        string ViewName { get; set; }

        Task<IViewAdapterResult> ConfigureAsync(string viewName);
    }
    
}
