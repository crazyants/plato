using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelViewProvider : BaseViewProvider<Channel>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<CategoryBase> _categoryStore;
        private readonly ICategoryManager<CategoryBase> _categoryManager;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<CategoryBase> categoryStore,
            ICategoryManager<CategoryBase> categoryManager)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Channel channel, IUpdateModel updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);

            CategoryBase categoryBase = null;
            if (channel.Id > 0)
            {
                categoryBase = await _categoryStore.GetByIdAsync(channel.Id);
            }

            // filter options
            var filterOptions = new FilterOptions
            {
                ChannelId = categoryBase?.Id ?? 0
            };

            // paging otptions
            var pagerOptions = new PagerOptions
            {
                Page = GetPageIndex(updater)
            };

            var indexViewModel = new ChannelIndexViewModel
            {
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions
            };

            return Views(
                View<CategoryBase>("Home.Index.Header", model => categoryBase).Zone("header").Order(1),
                View<CategoryBase>("Home.Index.Tools", model => categoryBase).Zone("tools").Order(1),
                View<ChannelIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<ChannelsViewModel>("Discuss.Channels.Index.Sidebar", model =>
                {
                    model.SelectedChannelId = channel?.Id ?? 0;
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Channel indexViewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Channel category, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Channel category, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        #endregion

        #region "Private Methods"

        int GetPageIndex(IUpdateModel updater)
        {

            var page = 1;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }


        #endregion

    }
}
