using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Layout.ModelBinding;
using Plato.Layout.Views;

namespace Plato.Layout.Drivers
{

    public interface IViewProviderManager<in TModel> where TModel : class
    {
        Task<CompiledViewProvider> BuildDisplayAsync(TModel model, IUpdateModel updater);

        Task<CompiledViewProvider> BuildEditAsync(TModel model, IUpdateModel updater);

        Task<CompiledViewProvider> BuildUpdateAsync(TModel model, IUpdateModel updater);

    }

    public class ViewProviderManager<TModel> : IViewProviderManager<TModel> where TModel : class
    {

        private readonly IEnumerable<IViewProvider<TModel>> _providers;

        public ViewProviderManager(
            IEnumerable<IViewProvider<TModel>> providers)
        {
            _providers = providers;
        }


        public async Task<CompiledViewProvider> BuildDisplayAsync(TModel model, IUpdateModel updater)
        {

            var output = new CompiledViewProvider();
            var views = new List<IGenericView>();

            foreach (var provider in _providers)
            {
                var viewProviderResult = await provider.Display(model, updater);

              
                //if (result != null)
                //{
                //    //await result.ApplyAsync(context);
                //}
            }

            return null;
        }

        public Task<CompiledViewProvider> BuildEditAsync(TModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }

        public Task<CompiledViewProvider> BuildUpdateAsync(TModel model, IUpdateModel updater)
        {
            throw new NotImplementedException();
        }
    }
}
