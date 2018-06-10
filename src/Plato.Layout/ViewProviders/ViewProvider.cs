using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.ViewProviders
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildLayoutAsync(TModel model, IUpdateModel updater);
        
        Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

    }

}
