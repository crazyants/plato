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
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.Services;
using Plato.Articles.Categories.ViewModels;
using Plato.Articles.Models;
using Plato.Articles.Services;
using Plato.Categories.Services;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Features;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        private const string CategoryHtmlName = "category";

        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IPostManager<Article> _entityManager;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;
        private readonly IFeatureFacade _featureFacade;
    
        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public ArticleViewProvider(
            IStringLocalizer stringLocalizer,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IEntityCategoryManager entityCategoryManager,
            IHttpContextAccessor httpContextAccessor,
            ICategoryStore<Category> categoryStore,
            IBreadCrumbManager breadCrumbManager,
            IPostManager<Article> entityManager,
            IEntityStore<Article> entityStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
       
            _request = httpContextAccessor.HttpContext.Request;
            _categoryDetailsUpdater = categoryDetailsUpdater;
            _entityCategoryManager = entityCategoryManager;
            _entityCategoryStore = entityCategoryStore;
            _breadCrumbManager = breadCrumbManager;
            _featureFacade = featureFacade;
            _entityManager = entityManager;
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _entityStore = entityStore;

            T = stringLocalizer;
            S = stringLocalizer;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Article viewModel, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await GetFeatureAsync();
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categoryIndexViewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                }
            };

            return Views(
                View<CategoryIndexViewModel>("Article.Categories.Index.Content", model => categoryIndexViewModel).Zone("content")
                    .Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base controller 
            IEnumerable<Category> parents = null;
            if (entity.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(entity.CategoryId);
            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], home => home
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], channels => channels
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[entity.Title]);

            });
            
            // Get current feature
            var feature = await GetFeatureAsync();
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            return default(IViewProviderResult);

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext updater)
        {

            // Get feature
             var feature = await GetFeatureAsync();

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Override breadcrumb configuration within base controller 
            IEnumerable<Category> parents = null;
            if (entity.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(entity.CategoryId);
            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Articles"], home => home
                    .Action("Index", "Home", "Plato.Articles")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], category => category
                            .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a title
                if (!String.IsNullOrEmpty(entity.Title))
                {
                    builder.Add(S[entity.Title], t => t
                        .Action("Display", "Home", "Plato.Articles", new RouteValueDictionary
                        {
                            ["opts.id"] = entity.Id,
                            ["opts.alias"] = entity.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[entity.Id > 0 ? "Edit Article" : "New Article"]);

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
                View<CategoryDropDownViewModel>("Articles.Categories.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Article entity, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeModelAsync(Article entity, IUpdateModel updater)
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
                    foreach (var channelId in categoriesToAdd)
                    {
                        if (channelId > 0)
                        {
                            entity.CategoryId = channelId;
                        }
                    }
                }
            }

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
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {

                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(article))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(article.Id, categoryId);
                        if (entityCategory != null)
                        {
                            await _entityCategoryManager.DeleteAsync(entityCategory);
                        }
                    }

                    // Get current user
                    var user = await _contextFacade.GetAuthenticatedUserAsync();

                    // Add new entity category relationships
                    foreach (var categoryId in categoriesToAdd)
                    {
                        // Ensure relationship does not already exist
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(article.Id, categoryId);
                        if (entityCategory == null)
                        {
                            // Add relationship
                            await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = article.Id,
                                CategoryId = categoryId,
                                CreatedUserId = user?.Id ?? 0,
                                ModifiedUserId = user?.Id ?? 0,
                            });
                        }
                    }

                    ////// Update entity with first found category 
                    ////foreach (var id in categoriesToAdd)
                    ////{
                    ////    article.CategoryId = id;
                    ////    await _entityStore.UpdateAsync(article);
                    ////    break;
                    ////}

                    // Update added category meta data
                    foreach (var id in categoriesToAdd)
                    {
                        await _categoryDetailsUpdater.UpdateAsync(id);
                    }

                    // Update removed category meta data
                    foreach (var id in categoriesToRemove)
                    {
                        await _categoryDetailsUpdater.UpdateAsync(id);
                    }

                }

            }
           
            return await BuildEditAsync(article, context);

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

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Article entity)
        {

            // When creating a new entity use the categoryId set on the entity
            if (entity.Id == 0)
            {
                if (entity.CategoryId > 0)
                {
                    // return empty collection for new entities
                    return new List<int>()
                    {
                        entity.CategoryId
                    };
                }

                return new List<int>();

            }

            var categories = await _entityCategoryStore.GetByEntityIdAsync(entity.Id);;
            if (categories != null)
            {
                return categories.Select(s => s.CategoryId).ToArray();
            }

            return new List<int>();

        }

        async Task<IShellFeature> GetFeatureAsync()
        {
            return await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
        }

        #endregion
        
    }

}
