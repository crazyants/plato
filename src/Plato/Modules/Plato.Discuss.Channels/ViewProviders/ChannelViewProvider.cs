using System.Linq;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ChannelViewProvider : BaseViewProvider<Channel>
    {

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _categoryStore;
        private readonly ICategoryManager<Channel> _categoryManager;

        public ChannelViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Channel> categoryStore,
            ICategoryManager<Channel> categoryManager)
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

            // channel filter options
            var channelViewOpts = new ViewOptions
            {
                ChannelId = categoryBase?.Id ?? 0
            };

            // topic filter options
            var topicViewOpts = new Discuss.ViewModels.ViewOptions
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
                ChannelViewOpts = channelViewOpts,
                TopicViewOpts = topicViewOpts,
                PagerOpts = pagerOptions
            };

            return Views(
                View<CategoryBase>("Home.Index.Header", model => categoryBase).Zone("header").Order(1),
                View<CategoryBase>("Home.Index.Tools", model => categoryBase).Zone("tools").Order(1),
                View<ChannelIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<ChannelListViewModel>("Topic.Channels.Index.Sidebar", model =>
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
