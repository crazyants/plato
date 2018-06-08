using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.ViewProviders
{
    
    public interface IViewProvider<in TModel> where TModel : class
    {

        Task<IViewProviderResult> DisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> EditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> UpdateAsync(TModel model, IUpdateModel updater);

    }

}
