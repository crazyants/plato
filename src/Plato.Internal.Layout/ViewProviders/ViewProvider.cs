using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> BuildDisplayAsync(TModel viewModel, IViewProviderContext context);

        Task<IViewProviderResult> BuildIndexAsync(TModel viewModel, IViewProviderContext context);
        
        Task<IViewProviderResult> BuildEditAsync(TModel viewModel, IViewProviderContext context);

        Task<IViewProviderResult> BuildUpdateAsync(TModel viewModel, IViewProviderContext context);

        Task<bool> ValidateModelAsync(TModel model, IUpdateModel updater);

        Task ComposeModelAsync(TModel model, IUpdateModel updater);

    }

}
