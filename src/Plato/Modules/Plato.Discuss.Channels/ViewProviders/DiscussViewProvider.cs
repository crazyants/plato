using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class DiscussViewProvider : BaseViewProvider<Topic>
    {

        private const string ChannelHtmlName = "channel";
        
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly ICategoryRoleStore<CategoryRole> _categoryRoleStore;

        public DiscussViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Channel> channelStore, 
            ICategoryRoleStore<CategoryRole> categoryRoleStore)
        {
            _contextFacade = contextFacade;
            _channelStore = channelStore;
            _categoryRoleStore = categoryRoleStore;
        }
        
        public override async Task<IViewProviderResult> BuildIndexAsync(Topic viewModel, IUpdateModel updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic viewModel, IUpdateModel updater)
        {

            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<ChannelsViewModel>("Discuss.Index.Sidebar", model =>
                {
                    model.Channels = categories;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }


        public override async Task<IViewProviderResult> BuildEditAsync(Topic topic, IUpdateModel updater)
        {

           // var selectedchannels = _categoryRoleStore.GetByCategoryIdAsync()
            //var selectedChannels = await _channelStore.GetByFeatureIdAsync(feature.Id);

            return Views(View<EditTopicChannelsViewModel>("Discuss.Edit.Sidebar", model =>
                {
                    model.HtmlName = ChannelHtmlName;
                    model.SelectedChannels = new List<int>();
                    return model;
                }).Zone("sidebar").Order(1)
            );



        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Topic viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }
}
