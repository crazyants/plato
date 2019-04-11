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
using Plato.Questions.Categories.Models;
using Plato.Questions.Categories.Services;
using Plato.Questions.Models;
using Plato.Questions.Services;
using Plato.Entities.Stores;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Questions.Categories.ViewProviders
{
    public class QuestionViewProvider : BaseViewProvider<Question>
    {

        private const string CategoryHtmlName = "category";
        
        private readonly IEntityCategoryStore<EntityCategory> _entityCategoryStore;
        private readonly IEntityCategoryManager _entityCategoryManager;
        private readonly ICategoryDetailsUpdater _categoryDetailsUpdater;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly IEntityStore<Question> _entityStore;
        private readonly IPostManager<Question> _entityManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly HttpRequest _request;

        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public QuestionViewProvider(
            IStringLocalizer stringLocalizer,
            IEntityCategoryStore<EntityCategory> entityCategoryStore,
            IEntityCategoryManager entityCategoryManager,
            ICategoryDetailsUpdater categoryDetailsUpdater,
            IHttpContextAccessor httpContextAccessor,
            ICategoryStore<Category> categoryStore, 
            IBreadCrumbManager breadCrumbManager,
            IPostManager<Question> entityManager,
            IEntityStore<Question> entityStore,
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

        public override async Task<IViewProviderResult> BuildIndexAsync(Question viewModel, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            return Views(View<CategoryListViewModel<CategoryAdmin>>("Question.Categories.Index.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.ParentId == 0);
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Question question, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
            if (question.CategoryId > 0)
            {
                parents = await _categoryStore.GetParentsByIdAsync(question.CategoryId);
            }

            _breadCrumbManager.Configure(builder =>
            {

                builder.Add(S["Home"], home => home
                    .Action("Index", "Home", "Plato.Core")
                    .LocalNav()
                ).Add(S["Questions"], home => home
                    .Action("Index", "Home", "Plato.Questions")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], channels => channels
                        .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary
                            {
                                ["opts.categoryId"] = parent.Id,
                                ["opts.alias"] = parent.Alias,
                            })
                            .LocalNav()
                        );
                    }
                }

                builder.Add(S[question.Title]);

            });
            
            // Get current feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions.Categories");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            // Get all categories & return views
            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<CategoryListViewModel<CategoryAdmin>>("Question.Categories.Display.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.Id == question.CategoryId);
                    return model;
                }).Zone("sidebar").Order(2)
            );

        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(Question entity, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions.Categories");

            // Ensure we found the feature
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Override breadcrumb configuration within base discuss controller 
            IEnumerable<CategoryAdmin> parents = null;
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
                    .Action("Index", "Home", "Plato.Questions")
                    .LocalNav()
                );

                if (parents != null)
                {
                    builder.Add(S["Categories"], categories => categories
                        .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary()
                        {
                            ["opts.categoryId"] = null,
                            ["opts.alias"] = null
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], category => category
                            .Action("Index", "Home", "Plato.Questions.Categories", new RouteValueDictionary
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
                        .Action("Display", "Home", "Plato.Questions", new RouteValueDictionary
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
                View<CategoryDropDownViewModel>("Question.Categories.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Question topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeTypeAsync(Question question, IUpdateModel updater)
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
                            question.CategoryId = categoryId;
                        }
                    }
                }
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Question question, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(question.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(question, context);
            }

            // Validate model
            if (await ValidateModelAsync(question, context.Updater))
            {
               
                // Get selected categories
                var categoriesToAdd = GetCategoriesToAdd();
                if (categoriesToAdd != null)
                {
                    
                    // Build categories to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var categoryId in await GetCategoryIdsByEntityIdAsync(question))
                    {
                        if (!categoriesToAdd.Contains(categoryId))
                        {
                            categoriesToRemove.Add(categoryId);
                        }
                    }

                    // Remove categories
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(question.Id, categoryId);
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
                        var entityCategory = await _entityCategoryStore.GetByEntityIdAndCategoryIdAsync(question.Id, categoryId);
                        if (entityCategory == null)
                        {
                            // Add relationship
                            await _entityCategoryManager.CreateAsync(new EntityCategory()
                            {
                                EntityId = question.Id,
                                CategoryId = categoryId,
                                CreatedUserId = user?.Id ?? 0,
                                ModifiedUserId = user?.Id ?? 0,
                            });
                        }
                    }
                    
                    // Update entity with first found category 
                    foreach (var id in categoriesToAdd)
                    {
                        question.CategoryId = id;
                        await _entityStore.UpdateAsync(question);
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
           
            return await BuildEditAsync(question, context);

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

        async Task<IEnumerable<int>> GetCategoryIdsByEntityIdAsync(Question entity)
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
