using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string ChannelHtmlName = "channel";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly HttpRequest _request;
        private readonly IFeatureFacade _featureFacade;
    
        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public TopicViewProvider(
            IStringLocalizer<TopicViewProvider> stringLocalizer,
            IContextFacade contextFacade,
            ICategoryStore<Channel> channelStore, 
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _channelStore = channelStore;
            _entityStore = entityStore;
            _entityCategoryStore = entityCategoryStore;
            _request = httpContextAccessor.HttpContext.Request;
            _breadCrumbManager = breadCrumbManager;
            _featureFacade = featureFacade;

            T = stringLocalizer;
            S = stringLocalizer;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Topic viewModel, IViewProviderContext updater)
        {

            // Ensure we explictly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            return Views(View<ChannelListViewModel>("Topic.Channels.Index.Sidebar", model =>
                {
                    model.Channels = categories?.Where(c => c.ParentId == 0);
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<Channel> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _channelStore.GetParentsByIdAsync(topic.CategoryId);
            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary()
                        {
                            ["id"] = "",
                            ["alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
                            {
                                ["id"] = parent.Id,
                                ["alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[topic.Title]);

            });
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get all categories & return views
            var categories = await _channelStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<ChannelListViewModel>("Topic.Channels.Display.Sidebar", model =>
                {
                    model.Channels = categories?.Where(c => c.Id == topic.CategoryId);
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext updater)
        {
            
            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<Channel> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _channelStore.GetParentsByIdAsync(topic.CategoryId);

            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Discuss"], home => home
                    .Action("Index", "Home", "Plato.Discuss")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary()
                        {
                            ["id"] = "",
                            ["alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
                            {
                                ["id"] = parent.Id,
                                ["alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a topic title
                if (!String.IsNullOrEmpty(topic.Title))
                {
                    builder.Add(S[topic.Title], t => t
                        .Action("Topic", "Home", "Plato.Discuss", new RouteValueDictionary
                        {
                            ["id"] = topic.Id,
                            ["alias"] = topic.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[topic.Id > 0 ? "Edit Post" : "New Post"]);

            });
            
            var viewModel = new EditTopicChannelsViewModel()
            {
                HtmlName = ChannelHtmlName,
                SelectedChannels = await GetCategoryIdsByEntityIdAsync(topic)
            };

            return Views(
                View<EditTopicChannelsViewModel>("Topic.Channels.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
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
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, context);
            }

            // Validate model
            if (await ValidateModelAsync(topic, context.Updater))
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
                    
                    // Get current user
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

                    // Update primary category
                    foreach (var channelId in channelsToAdd)
                    {
                        if (channelId > 0)
                        {
                            topic.CategoryId = channelId;
                            await _entityStore.UpdateAsync(topic);
                            break;
                        }

                    }

                }

            }
           
            return await BuildEditAsync(topic, context);

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
