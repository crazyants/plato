using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Ideas.Categories.Models;
using Plato.Ideas.Categories.Services;
using Plato.Ideas.Models;
using Plato.Ideas.Services;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Ideas.Categories.ViewProviders
{
    public class IdeaViewProvider : BaseViewProvider<Idea>
    {

        private const string CategoryHtmlName = "category";
        
        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Idea> _entityStore;
        private readonly IPostManager<Idea> _entityManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public IdeaViewProvider(
            IStringLocalizer stringLocalizer,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IEntityCategoryManager entityCategoryManager,
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IHttpContextAccessor httpContextAccessor,
            ICategoryStore<Category> categoryStore, 
            IBreadCrumbManager breadCrumbManager,
            IPostManager<Idea> entityManager,
            IEntityStore<Idea> entityStore,
            IFeatureFacade featureFacade,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityCategoryManager = entityCategoryManager;
            _categoryDetailsUpdater = categoryDetailsUpdater;
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

        public override async Task<IViewProviderResult> BuildIndexAsync(Idea viewModel, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            return Views(View<CategoryListViewModel<Category>>("Idea.Categories.Index.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.ParentId == 0);
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Idea idea, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (idea.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(idea.CategoryId);
            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Ideas"], home => home
                    .Action("Index", "Home", "Plato.Ideas")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], channels => channels
                        .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[idea.Title]);

            });
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get all categories & return views
            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<CategoryListViewModel<Category>>("Idea.Categories.Display.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.Id == idea.CategoryId);
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Idea idea, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (idea.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(idea.CategoryId);
            }
            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Ideas"], home => home
                    .Action("Index", "Home", "Plato.Ideas")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], category => category
                            .Action("Index", "Home", "Plato.Ideas.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                // Ensure we have a title
                if (!String.IsNullOrEmpty(idea.Title))
                {
                    builder.Add(S[idea.Title], t => t
                        .Action("Display", "Home", "Plato.Ideas", new RouteValueDictionary
                        {
                            ["opts.id"] = idea.Id,
                            ["opts.alias"] = idea.Alias,
                        })
                        .LocalNav()
                    );
                }
           
                builder.Add(S[idea.Id > 0 ? "Edit Idea" : "New Idea"]);

            });
            
            var viewModel = new CategoryDropDownViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id
                },
                HtmlName = CategoryHtmlName,
                SelectedCategories = await GetCategoryIdsByEntityIdAsync(idea)
            };

            return Views(
                View<CategoryDropDownViewModel>("Idea.Categories.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Idea idea, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeTypeAsync(Idea idea, IUpdateModel updater)
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
                            idea.CategoryId = categoryId;
                        }
                    }
                }
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Idea idea, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(idea.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(idea, context);
            }

            // Validate model
            if (await ValidateModelAsync(idea, context.Updater))
            {
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {
                    
                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(idea))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(idea.Id, categoryId);
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
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(idea.Id, categoryId);
                        if (entityCategory == null)
                        {
                            // Add relationship
                            await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = idea.Id,
                                CategoryId = categoryId,
                                CreatedUserId = user?.Id ?? 0,
                                ModifiedUserId = user?.Id ?? 0,
                            });
                        }
                    }
                    
                    // Update entity with first found category 
                    foreach (var id in categoriesToAdd)
                    {
                        idea.CategoryId = id;
                        await _entityStore.UpdateAsync(idea);
                        break;
                    }
                    
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
           
            return await BuildEditAsync(idea, context);

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

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Idea entity)
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

        #endregion
        
    }

}
