using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{
    public interface IViewProviderManager<TModel> where TModel : class
    {

        Task<IViewProviderResult> ProvideDisplayAsync(TModel model, IUpdateModel context);

        Task<IViewProviderResult> ProvideIndexAsync(TModel model, IUpdateModel context);

        Task<IViewProviderResult> ProvideEditAsync(TModel model, IUpdateModel context);

        Task<IViewProviderResult> ProvideUpdateAsync(TModel model, IUpdateModel context);

        Task<bool> IsModelStateValid(TModel model, IUpdateModel updater);

        Task<TModel> GetComposedType(IUpdateModel updater);

    }

}
