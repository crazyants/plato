using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Articles.Models;
using Plato.Articles.Tags.Models;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;

namespace Plato.Articles.Tags.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {
        private const string TagsHtmlName = "tags";

        private readonly ITagStore<Tag> _tagStore;
        private readonly IEntityTagStore<EntityTag> _entityTagStore;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityTagManager<EntityTag> _entityTagManager;
        private readonly ITagManager<Tag> _tagManager;
        private readonly IFeatureFacade _featureFacade;
        private readonly IContextFacade _contextFacade;

        private readonly HttpRequest _request;
        
        public ArticleViewProvider(
            ITagStore<Tag> tagStore,
            IEntityStore<Article> entityStore,
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
        
        public override async Task<IViewProviderResult> BuildIndexAsync(Article article, IViewProviderContext context)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
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
                View<TagsViewModel<Tag>>("Article.Tags.Index.Sidebar", model =>
                {
                    model.Tags = tags?.Data;
                    return model;
                }).Zone("sidebar").Order(4)
            );

        }


        public override Task<IViewProviderResult> BuildDisplayAsync(Article article, IViewProviderContext context)
        {
            return Task.FromResult(Views(
                View<EditEntityTagsViewModel>("Article.Tags.Edit.Footer", model => new EditEntityTagsViewModel()
                    {
                        HtmlName = TagsHtmlName,
                        Permission = Permissions.PostArticleCommentTags
                    }).Zone("footer")
                    .Order(int.MaxValue)
            ));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article article, IViewProviderContext context)
        {
            
            var tagsJson = "";
            
            // Ensures we persist the tag json between post backs
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

                var entityTags = await GetEntityTagsByEntityIdAsync(article.Id);

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

            return Views(
                View<EditEntityTagsViewModel>("Article.Tags.Edit.Footer", model => new EditEntityTagsViewModel()
                    {
                        Tags = tagsJson,
                        HtmlName = TagsHtmlName,
                        Permission = article.Id == 0
                            ? Permissions.PostArticleTags
                            : Permissions.EditArticleTags
                    }).Zone("content")
                    .Order(int.MaxValue)
            );

        }
        
        public override Task<bool> ValidateModelAsync(Article article, IUpdateModel updater)
        {
            // ensure tags are optional
            return Task.FromResult(true);
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext context)
        {
            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(article.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(article, context);
            }

            // Validate model
            if (await ValidateModelAsync(article, context.Updater))
            {

                // Get selected tags
                var tagsToAdd = await GetTagsToAddAsync(article);

                // Build tags to remove
                var tagsToRemove = new List<EntityTag>();

                // Get all existing tags for entity
                var existingTags = await GetEntityTagsByEntityIdAsync(article.Id);

                // Exclude replies
                var existingTagsList = existingTags?.Where(t => t.EntityReplyId == 0).ToList();
                
                // Iterate over existing tags reducing our tags to add
                if (existingTagsList != null)
                {
                    foreach (var entityTag in existingTagsList)
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

                // Add new entity tags
                foreach (var tag in tagsToAdd)
                {
                    var result = await _entityTagManager.CreateAsync(new EntityTag()
                    {
                        EntityId = article.Id,
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

            return await BuildEditAsync(article, context);

        }

        #endregion

        #region "Private Methods"

        async Task<List<TagBase>> GetTagsToAddAsync(Article article)
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
                                    var newTag = await CreateTag(item.Name);
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

        async Task<TagBase> CreateTag(string name)
        {

            // Current user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // We need to be authenticated to add tags
            if (user == null)
            {
                return null;
            }

            // Get feature for tag
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
      
            // Create tag
            var result = await _tagManager.CreateAsync(new Tag()
            {
                FeatureId = feature?.Id ?? 0,
                Name = name,
                CreatedUserId = user.Id,
                CreatedDate = DateTimeOffset.UtcNow
            });
            
            if (result.Succeeded)
            {
                return result.Response;
            }

            return null;

        }

        async Task<IEnumerable<EntityTag>> GetEntityTagsByEntityIdAsync(int entityId)
        {

            if (entityId == 0)
            {
                // return null for new entities
                return null;
            }
            
            return await _entityTagStore.GetByEntityIdAsync(entityId);

        }

        #endregion

    }

}
