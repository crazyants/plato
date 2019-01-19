using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {
        private const string TagsHtmlName = "tags";

        private readonly ITagStore<Tag> _tagStore;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityTagManager<EntityTag> _entityTagManager;
        private readonly ITagManager<Tag> _tagManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;

        private readonly HttpRequest _request;
        
        public TopicViewProvider(
            ITagStore<Tag> tagStore,
            IEntityStore<Topic> entityStore,
            IEntityTagStore<EntityTag> entityTagStore,
            IHttpContextAccessor httpContextAccessor, 
            IEntityTagManager<EntityTag> entityTagManager,
            ITagManager<Tag> tagManager, 
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _tagStore = tagStore;
            _entityStore = entityStore;
            _entityTagStore = entityTagStore;
            _entityTagManager = entityTagManager;
            _tagManager = tagManager;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"
        
        public override async Task<IViewProviderResult> BuildIndexAsync(Topic viewModel, IViewProviderContext context)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get tags
            var tags = await _tagStore.QueryAsync()
                .Take(1, 20)
                .Select<TagQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            return Views(
                View<TagsViewModel>("Topic.Tags.Index.Sidebar", model =>
                {
                    model.Tags = tags?.Data;
                    return model;
                }).Zone("sidebar").Order(4)
            );

        }


        public override Task<IViewProviderResult> BuildDisplayAsync(Topic viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext context)
        {
            
            var tagsJson = "";
            
            // Ensures we persist the tag json between post backs
            var message = topic.Message;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == TagsHtmlName)
                    {
                        tagsJson = _request.Form[key];
                    }
                }
            }
            else
            {

                var entityTags = await GetEntityTagsByEntityIdAsync(topic.Id);

                // Exclude replies
                var entityTagList = entityTags?.Where(t => t.EntityReplyId == 0).ToList();

                if (entityTagList?.Count > 0)
                {

                    // Get entity tags
                    var tags = await _tagStore.QueryAsync()
                        .Select<TagQueryParams>(q =>
                        {
                            q.Id.IsIn(entityTagList.Select(e => e.TagId).ToArray());
                        })
                        .OrderBy("Name")
                        .ToList();

                    List<TagApiResult> tagsToSerialize = null;
                    if (tags != null)
                    {
                        tagsToSerialize = new List<TagApiResult>();
                        foreach (var tag in tags.Data)
                        {
                            tagsToSerialize.Add(new TagApiResult()
                            {
                                Id = tag.Id,
                                Name = tag.Name
                            });
                        }
                    }

                    if (tagsToSerialize != null)
                    {
                        tagsJson = tagsToSerialize.Serialize();
                    }

                }

            }
            
            var viewModel = new EditTopicTagsViewModel()
            {
                Tags = tagsJson,
                HtmlName = TagsHtmlName
            };

            return Views(
                View<EditTopicTagsViewModel>("Topic.Tags.Edit.Footer", model => viewModel).Zone("content")
                    .Order(int.MaxValue)
            );
            
        }
        
        public override Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
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

                // Get selected tags
                var tagsToAdd = await GetTagsToAddAsync(topic);

                // Build tags to remove
                var tagsToRemove = new List<EntityTag>();

                // Iterate over existing tags
                var existingTags = await GetEntityTagsByEntityIdAsync(topic.Id);
                if (existingTags != null)
                {
                    foreach (var entityTag in existingTags)
                    {
                        // Is our existing tag in our list of tags to add
                        var existingTag = tagsToAdd.FirstOrDefault(t => t.Id == entityTag.TagId);
                        if (existingTag != null)
                        {
                            tagsToAdd.Remove(existingTag);
                        }
                        else
                        {
                            // Entry no longer exist in tags so ensure it's removed
                            tagsToRemove.Add(entityTag);
                        }
                    }
                }
            
                // Remove entity tags
                foreach (var entityTag in tagsToRemove)
                {
                    var result = await _entityTagManager.DeleteAsync(entityTag);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }



                // Get authenticated user
                var user = await _contextFacade.GetAuthenticatedUserAsync();

                // Add new entity labels
                foreach (var tag in tagsToAdd)
                {
                    var result = await _entityTagManager.CreateAsync(new EntityTag()
                    {
                        EntityId = topic.Id,
                        TagId = tag.Id,
                        CreatedUserId = user?.Id ?? 0,
                        CreatedDate = DateTime.UtcNow
                    });
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

            }

            return await BuildEditAsync(topic, context);

        }

        #endregion

        #region "Private Methods"

        async Task<List<Tag>> GetTagsToAddAsync(Topic topic)
        {
         
            var tagsToAdd = new List<Tag>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.Equals(TagsHtmlName))
                {
                    var value = _request.Form[key];
                    if (!String.IsNullOrEmpty(value))
                    {

                        var items = JsonConvert.DeserializeObject<IEnumerable<TagApiResult>>(value);
                        foreach (var item in items)
                        {

                            // Get existing tag if we have an identity
                            if (item.Id > 0)
                            {
                                var tag = await _tagStore.GetByIdAsync(item.Id);
                                if (tag != null)
                                {
                                    tagsToAdd.Add(tag);
                                }
                            }
                            else
                            {

                                // Does the tag already exist by name?
                                var existingTag = await _tagStore.GetByNameNormalizedAsync(item.Name.Normalize());
                                if (existingTag != null)
                                {
                                    tagsToAdd.Add(existingTag);
                                }
                                else
                                {
                                    // Create tag
                                    var newTag = await CreateTag(item.Name, topic.Id);
                                    if (newTag != null)
                                    {
                                        tagsToAdd.Add(newTag);
                                    }
                                }
                            }

                        }

                    }

                }

            }

            return tagsToAdd;

        }

        async Task<Tag> CreateTag(string name, int entityId)
        {

            // Get feature for tag
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
      
            // Create tag
            var tagManagerResult = await _tagManager.CreateAsync(new Tag()
            {
                FeatureId = feature?.Id ?? 0,
                Name = name
            });
            
            if (tagManagerResult.Succeeded)
            {
                return tagManagerResult.Response;
            }

            return null;

        }

        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return null;
            }
            
            return await _entityTagStore.GetByEntityId(entityId);

        }

        #endregion

    }

}
