using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> BuildDisplayAsync(TModel model, IViewProviderContext context);

        Task<IViewProviderResult> BuildIndexAsync(TModel model, IViewProviderContext context);
        
        Task<IViewProviderResult> BuildEditAsync(TModel model, IViewProviderContext context);

        Task<IViewProviderResult> BuildUpdateAsync(TModel model, IViewProviderContext context);

        Task<bool> ValidateModelAsync(TModel model, IUpdateModel updater);

        Task ComposeTypeAsync(TModel model, IUpdateModel updater);

    }

}
