using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{
    public interface IViewProviderManager<in TModel> where TModel : class
    {

        Task<IViewProviderResult> ProvideDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideIndexAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideEditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> ProvideUpdateAsync(TModel model, IUpdateModel updater);

        Task<bool> IsModelStateValid(TModel model, IUpdateModel updater);

    }
    
}
