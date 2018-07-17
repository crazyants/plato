using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelViewProvider : BaseViewProvider<ChannelViewModel>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            ICategoryManager<Category> categoryManager)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(ChannelViewModel viewModel, IUpdateModel updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<ChannelViewModel>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<ChannelViewModel>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<ChannelViewModel>("Home.Index.Content", model => viewModel).Zone("content").Order(1),
                View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(ChannelViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(ChannelViewModel category, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(ChannelViewModel category, IUpdateModel updater)
        {

            return Task.FromResult(default(IViewProviderResult));


        }

        #endregion

        #region "Private Methods"

    
        #endregion

    }
}
