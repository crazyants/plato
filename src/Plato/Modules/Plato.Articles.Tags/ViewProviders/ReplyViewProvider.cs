using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;

namespace Plato.Articles.Tags.ViewProviders
{
    public class ReplyViewProvider : BaseViewProvider<Comment>
    {

        private const string TagsHtmlName = "tags";


        private readonly IEntityReplyStore<Comment> _replyStore;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityTagManager<EntityTag> _entityTagManager;
        private readonly ITagStore<TagBase> _tagStore;
        private readonly ITagManager<TagBase> _tagManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;

        private readonly IStringLocalizer T;

        private readonly HttpRequest _request;

        public ReplyViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer stringLocalize,
            
            IEntityReplyStore<Comment> replyStore,
            IEntityTagStore<EntityTag> entityTagStore, 
            ITagStore<TagBase> tagStore, 
            IEntityTagManager<EntityTag> entityTagManager,
            ITagManager<TagBase> tagManager,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _replyStore = replyStore;
            _entityTagStore = entityTagStore;
            _tagStore = tagStore;
            _entityTagManager = entityTagManager;
            _tagManager = tagManager;
            _featureFacade = featureFacade;
            _contextFacade = contextFacade;
            _request = httpContextAccessor.HttpContext.Request;

            T = stringLocalize;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildDisplayAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Comment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Comment reply, IViewProviderContext updater)
        {

            var tagsJson = "";
            var entityTags = await GetEntityTagsByEntityReplyIdAsync(reply.Id);
            if (entityTags != null)
            {

                var tags = await _tagStore.QueryAsync()
                    .Select<TagQueryParams>(q =>
                    {
                        q.Id.IsIn(entityTags.Select(e => e.TagId).ToArray());
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

            var viewModel = new EditEntityTagsViewModel()
            {
                Tags = tagsJson,
                HtmlName = TagsHtmlName
            };

            return Views(
                View<EditEntityTagsViewModel>("Article.Tags.Edit.Footer", model => viewModel).Zone("content")
                    .Order(int.MaxValue)
            );


        }

        public override Task<bool> ValidateModelAsync(Comment reply, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Comment reply, IViewProviderContext context)
        {

            // Ensure entity reply exists before attempting to update
            var entity = await _replyStore.GetByIdAsync(reply.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Validate model
            if (await ValidateModelAsync(reply, context.Updater))
            {

                // Get selected tags
                var tagsToAdd = await GetTagsToAddAsync(reply);

                // Build tags to remove
                var tagsToRemove = new List<EntityTag>();

                // Iterate over existing tags
                var existingTags = await GetEntityTagsByEntityReplyIdAsync(reply.Id);
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
                        EntityId = reply.EntityId,
                        EntityReplyId = reply.Id,
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

            return await BuildEditAsync(reply, context);

        }

        #endregion
        
        #region "Private Methods"

        async Task<List<TagBase>> GetTagsToAddAsync(Comment reply)
        {

            var tagsToAdd = new List<TagBase>();
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
                                    var newTag = await CreateTag(item.Name, reply.EntityId, reply.Id);
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

        async Task<TagBase> CreateTag(string name, int entityId, int replyId)
        {

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Get feature for tag
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Create tag
            var tagManagerResult = await _tagManager.CreateAsync(new TagBase()
            {
                FeatureId = feature?.Id ?? 0,
                Name = name,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTime.UtcNow
            });

            if (tagManagerResult.Succeeded)
            {
                return tagManagerResult.Response;
            }

            return null;

        }


        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityReplyIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return empty collection for new topics
                return null;
            }

            return await _entityTagStore.GetByEntityReplyId(entityId);

        }

        #endregion
        
    }

}
