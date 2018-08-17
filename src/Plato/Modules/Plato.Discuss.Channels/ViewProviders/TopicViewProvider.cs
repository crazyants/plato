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
using Plato.Discuss.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string ChannelHtmlName = "channel";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IStringLocalizer T;
        private readonly IPostManager<Topic> _topicManager;

        private readonly HttpRequest _request;

        public TopicViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Channel> channelStore, 
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IStringLocalizer<TopicViewProvider> stringLocalize,
            IPostManager<Topic> topicManager)
        {
            _contextFacade = contextFacade;
            _channelStore = channelStore;
            _entityStore = entityStore;
            _entityCategoryStore = entityCategoryStore;
            _request = httpContextAccessor.HttpContext.Request;
            T = stringLocalize;
            _topicManager = topicManager;
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
            
            return Views(View<ChannelListViewModel>("Discuss.Channels.Index.Sidebar", model =>
                {
                    model.Channels = categories.Where(c => c.ParentId == 0);
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
                View<ChannelListViewModel>("Discuss.Channels.Index.Sidebar", model =>
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
                SelectedChannels = await GetCategoryIdsByEntityIdAsync(topic)
            };

            return Views(
                View<EditTopicChannelsViewModel>("Discuss.Channels.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditTopicChannelsViewModel
            {
                SelectedChannels = GetChannelsToAdd()
            });
        }

        public override async Task ComposeTypeAsync(Topic topic, IUpdateModel updater)
        {

            var model = new EditTopicChannelsViewModel
            {
                SelectedChannels = GetChannelsToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                var channelsToAdd = GetChannelsToAdd();
                if (channelsToAdd != null)
                {
                    foreach (var channelId in channelsToAdd)
                    {
                        if (channelId > 0)
                        {
                            topic.CategoryId = channelId;
                        }
                    }
                }
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IUpdateModel updater)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, updater);
            }

            // Validate model
            if (await ValidateModelAsync(topic, updater))
            {
               
                // Get selected channels
                var channelsToAdd = GetChannelsToAdd();
                if (channelsToAdd != null)
                {

                    // Build channels to remove
                    var channelsToRemove = new List<int>();
                    foreach (var channel in await GetCategoryIdsByEntityIdAsync(topic))
                    {
                        if (!channelsToAdd.Contains(channel))
                        {
                            channelsToRemove.Add(channel);
                        }
                    }

                    // Remove channels
                    foreach (var channelId in channelsToRemove)
                    {
                        await _entityCategoryStore.DeleteByEntityIdAndCategoryId(topic.Id, channelId);
                    }
                    
                    var user = await _contextFacade.GetAuthenticatedUserAsync();

                    // Add new entity category relationship
                    foreach (var channelId in channelsToAdd)
                    {
                        await _entityCategoryStore.CreateAsync(new EntityCategory()
                        {
                            EntityId = topic.Id,
                            CategoryId = channelId,
                            CreatedUserId = user?.Id ?? 0,
                            ModifiedUserId = user?.Id ?? 0,
                        });
                    }

                    //// Update primary category
                    //foreach (var channelId in channelsToAdd)
                    //{
                    //    if (channelId > 0)
                    //    {
                    //        topic.CategoryId = channelId;
                    //        await _topicManager.UpdateAsync(topic);
                    //        break;
                    //    }
                      
                    //}
                    
                }

            }
           
            return await BuildEditAsync(topic, updater);

        }


        #endregion

        #region "Private Methods"
        
        List<int> GetChannelsToAdd()
        {
            // Build selected channels
            List<int> channelsToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ChannelHtmlName))
                {
                    if (channelsToAdd == null)
                    {
                        channelsToAdd = new List<int>();
                    }
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

            return channelsToAdd;
        }

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Topic entity)
        {

            // When creating a new topic use the categoryId set on the topic
            if (entity.Id == 0)
            {
                if (entity.CategoryId > 0)
                {
                    // return empty collection for new topics
                    return new List<int>()
                    {
                        entity.CategoryId
                    };
                }

                return new List<int>();

            }

            var channels = await _entityCategoryStore.GetByEntityId(entity.Id);;
            if (channels != null)
            {
                return channels.Select(s => s.CategoryId).ToArray();
            }

            return new List<int>();

        }

        #endregion



    }
}
