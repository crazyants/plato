using System.Threading.Tasks;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorProvider
    {
        
        string ViewName { get; }

        Task<IViewAdaptorResult> ConfigureAsync();
    }
    
}
