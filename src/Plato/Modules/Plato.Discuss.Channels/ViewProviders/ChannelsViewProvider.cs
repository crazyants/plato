using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelsViewProvider : BaseViewProvider<Category>
    {


        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;

        public ChannelsViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
        }


        public override async Task<IViewProviderResult> BuildIndexAsync(Category viewModel, IUpdateModel updater)
        {
            var indexViewModel = await GetIndexModel();
         
            return Views(
                View<ChannelsViewModel>("Admin.Index.Header", model => indexViewModel).Zone("header").Order(1),
                View<ChannelsViewModel>("Admin.Index.Tools", model => indexViewModel).Zone("tools").Order(1),
                View<ChannelsViewModel>("Admin.Index.Content", model => indexViewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }


        public override async Task<IViewProviderResult> BuildEditAsync(Category viewModel, IUpdateModel updater)
        {

            EditChannelViewModel editChannelViewModel = null;
            if (viewModel.Id == 0)
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    ChannelIcons = new DefaultIcons()
                };
            }
            else
            {

            }


            return Views(
                View<EditChannelViewModel>("Admin.Edit.Header", model => editChannelViewModel).Zone("header").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Content", model => editChannelViewModel).Zone("content").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Actions", model => editChannelViewModel).Zone("actions").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Footer", model => editChannelViewModel).Zone("footer").Order(1)
            );
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Category viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }



        async Task<ChannelsViewModel> GetIndexModel()
        {

            var featureId = "Plato.Discuss.Channels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            return new ChannelsViewModel()
            {
                Channels = categories,
                EditChannel = new Category()
            };

        }


    }
}
