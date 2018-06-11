using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildIndexAsync(TModel model, IUpdateModel updater);
        
        Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

    }

}
