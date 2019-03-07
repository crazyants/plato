using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string CategoryHtmlName = "channel";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly HttpRequest _request;
        private readonly IFeatureFacade _featureFacade;
    
        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public TopicViewProvider(
            IStringLocalizer<TopicViewProvider> stringLocalizer,
            IContextFacade contextFacade,
            ICategoryStore<Channel> categoryStore, 
            IEntityStore<Topic> entityStore,
            IHttpContextAccessor httpContextAccessor,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IBreadCrumbManager breadCrumbManager,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
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

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            return Views(View<CategoryListViewModel<ChannelAdmin>>("Topic.Channels.Index.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.ParentId == 0);
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<ChannelAdmin> parents = null;
            if (topic.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(topic.CategoryId);
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
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
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
            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<CategoryListViewModel<ChannelAdmin>>("Topic.Channels.Display.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.Id == topic.CategoryId);
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Channels");

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }


            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<ChannelAdmin> parents = null;
            if (entity.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(entity.CategoryId);
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
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Discuss.Channels", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a topic title
                if (!String.IsNullOrEmpty(entity.Title))
                {
                    builder.Add(S[entity.Title], t => t
                        .Action("Display", "Home", "Plato.Discuss", new RouteValueDictionary
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[entity.Id > 0 ? "Edit Topic" : "New Topic"]);

            });
            
            var viewModel = new CategoryDropDownViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                },
                HtmlName = CategoryHtmlName,
                SelectedCategories = await GetCategoryIdsByEntityIdAsync(entity)
            };

            return Views(
                View<CategoryDropDownViewModel>("Topic.Channels.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeTypeAsync(Topic topic, IUpdateModel updater)
        {

            var model = new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {
                    foreach (var categoryId in categoriesToAdd)
                    {
                        if (categoryId > 0)
                        {
                            topic.CategoryId = categoryId;
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
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {

                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var category in await GetCategoryIdsByEntityIdAsync(topic))
                    {
                        if (!categoriesToAdd.Contains(category))
                        {
                            categoriesToRemove.Add(category);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        await _entityCategoryStore.DeleteByEntityIdAndCategoryId(topic.Id, categoryId);
                    }
                    
                    // Get current user
                    var user = await _contextFacade.GetAuthenticatedUserAsync();

                    // Add new entity category relationship
                    foreach (var categoryId in categoriesToAdd)
                    {
                        await _entityCategoryStore.CreateAsync(new EntityCategory()
                        {
                            EntityId = topic.Id,
                            CategoryId = categoryId,
                            CreatedUserId = user?.Id ?? 0,
                            ModifiedUserId = user?.Id ?? 0,
                        });
                    }

                    // Update primary category
                    foreach (var categoryId in categoriesToAdd)
                    {
                        if (categoryId > 0)
                        {
                            topic.CategoryId = categoryId;
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
        
        List<int> GetCategoriesToAdd()
        {
            // Build selected categories
            List<int> categoriesToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(CategoryHtmlName))
                {
                    if (categoriesToAdd == null)
                    {
                        categoriesToAdd = new List<int>();
                    }
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (!categoriesToAdd.Contains(id))
                        {
                            categoriesToAdd.Add(id);
                        }
                    }
                }
            }

            return categoriesToAdd;
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

            var categories = await _entityCategoryStore.GetByEntityId(entity.Id);;
            if (categories != null)
            {
                return categories.Select(s => s.CategoryId).ToArray();
            }

            return new List<int>();

        }

        #endregion
        
    }

}
