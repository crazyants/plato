using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;

namespace Plato.Layout.ViewProviders
{

    public interface IViewProviderManager<in TModel> where TModel : class
    {
        Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater);

        Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater);

    }

    public class ViewProviderManager<TModel> : IViewProviderManager<TModel> where TModel : class
    {

        private readonly IEnumerable<IViewProvider<TModel>> _providers;

        public ViewProviderManager(IEnumerable<IViewProvider<TModel>> providers)
        {
            _providers = providers;
        }
        
        public async Task<IViewProviderResult> BuildDisplayAsync(TModel model, IUpdateModel updater)
        {

            var results = new List<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                results.Add(await provider.DisplayAsync(model, updater));
            }

            return new CombinedViewProviderResult(results);

        }

        public async Task<IViewProviderResult> BuildEditAsync(TModel model, IUpdateModel updater)
        {

            var results = new List<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                results.Add(await provider.EditAsync(model, updater));
            }

            return new CombinedViewProviderResult(results);

        }

        public async Task<IViewProviderResult> BuildUpdateAsync(TModel model, IUpdateModel updater)
        {

            var results = new List<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                results.Add(await provider.UpdateAsync(model, updater));
            }

            return new CombinedViewProviderResult(results);

        }
    }
}
