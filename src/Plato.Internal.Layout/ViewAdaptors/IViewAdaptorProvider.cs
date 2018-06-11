using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdaptors
{

    public interface IViewAdaptorProvider
    {
        
        string ViewName { get; }

        Task<IViewAdaptorResult> ConfigureAsync();
    }
    
}
