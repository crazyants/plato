using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class DiscussViewProvider : BaseViewProvider<Topic>
    {

        private const string ChannelHtmlName = "channel";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public DiscussViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Channel> channelStore, 
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IStringLocalizer<DiscussViewProvider> stringLocalize)
        {
            _contextFacade = contextFacade;
            _channelStore = channelStore;
            _entityStore = entityStore;
            _entityCategoryStore = entityCategoryStore;
            _request = httpContextAccessor.HttpContext.Request;
            T = stringLocalize;
        }

        #region "Implementation"

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
            var viewModel = new EditTopicChannelsViewModel()
            {
                HtmlName = ChannelHtmlName,
                SelectedChannels = await GetCategoryIdsByEntityIdAsync(topic.Id)
            };

            return Views(
                View<EditTopicChannelsViewModel>("Discuss.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IUpdateModel updater)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, updater);
            }
            
            // Build selected channels
            var channelsToAdd = new List<int>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ChannelHtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (!channelsToAdd.Contains(id))
                        {
                            channelsToAdd.Add(id);
                        }

                    }
                }
            }

            // Build model
            var model = new EditTopicChannelsViewModel();
            model.SelectedChannels = channelsToAdd;

            // Validate model
            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(topic, updater);
            }

            if (updater.ModelState.IsValid)
            {

                // Build channels to remove
                var channelsToRemove = new List<int>();
                foreach (var role in await GetCategoryIdsByEntityIdAsync(topic.Id))
                {
                    if (!channelsToAdd.Contains(role))
                    {
                        channelsToRemove.Add(role);
                    }
                }

                // Remove channels
                foreach (var channelId in channelsToRemove)
                {
                    await _entityCategoryStore.DeleteByEntityIdAndCategoryId(topic.Id, channelId);
                }

                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // Add new entity categories
                foreach (var channel in channelsToAdd)
                {
                    await _entityCategoryStore.CreateAsync(new EntityCategory()
                    {
                        EntityId = topic.Id,
                        CategoryId = channel,
                        CreatedUserId = user?.Id ?? 0,
                        ModifiedUserId = user?.Id ?? 0,
                    });
                }
                

            }
            else
            {

                updater.ModelState.AddModelError(string.Empty, T["A channel is required! XXXXXXXXXXXXX"]);

                return await BuildDisplayAsync(topic, updater);

            }

            return await BuildEditAsync(topic, updater);

        }

        #endregion

        #region "Private Methods"


        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return new List<int>();
            }

            var channels = await _entityCategoryStore.GetByEntityId(entityId);
            ;
            if (channels != null)
            {
                return channels.Select(s => s.Id).ToArray();
            }

            return new List<int>();

        }

        #endregion



    }
}
