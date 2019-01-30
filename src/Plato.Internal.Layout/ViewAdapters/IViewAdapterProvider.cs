using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdapters
{

    public interface IViewAdapterProvider
    {
        
        string ViewName { get; }

        Task<IViewAdapterResult> ConfigureAsync();
    }
    
}
