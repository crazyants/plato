using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Discuss.ViewModels;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class DiscussViewProvider : BaseViewProvider<DiscussViewModel>
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;

        public DiscussViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
        }


        public override async Task<IViewProviderResult> BuildIndexAsync(DiscussViewModel viewModel, IUpdateModel updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(DiscussViewModel viewModel, IUpdateModel updater)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }


        public override Task<IViewProviderResult> BuildEditAsync(DiscussViewModel viewModel, IUpdateModel updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(DiscussViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }
}
