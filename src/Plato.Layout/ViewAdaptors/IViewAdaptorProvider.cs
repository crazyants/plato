using System.Threading.Tasks;

namespace Plato.Layout.ViewAdaptors
{

    public interface IViewAdaptorProvider
    {
        
        string ViewName { get; }

        Task<IViewAdaptorResult> ConfigureAsync();
    }
    
}
