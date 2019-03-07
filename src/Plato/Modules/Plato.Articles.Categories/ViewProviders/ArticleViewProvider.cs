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
using Plato.Articles.Models;
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
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<CategoryHome> _categoryStore;
        private readonly IBreadCrumbManager _breadCrumbManager;
        private readonly HttpRequest _request;
        private readonly IFeatureFacade _featureFacade;
    
        public IStringLocalizer T;

        public IStringLocalizer S { get; }
        
        public ArticleViewProvider(
            IStringLocalizer<ArticleViewProvider> stringLocalizer,
            IContextFacade contextFacade,
            ICategoryStore<CategoryHome> categoryStore, 
            IEntityStore<Article> entityStore,
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

        public override async Task<IViewProviderResult> BuildIndexAsync(Article viewModel, IViewProviderContext updater)
        {

            // Ensure we explicitly set the featureId
            var feature = await GetFeatureAsync();
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            return Views(View<CategoryListViewModel<CategoryHome>>("Article.Categories.Index.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.ParentId == 0);
                    return model;
                }).Zone("sidebar").Order(1)
            );
            

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {

            // Override breadcrumb configuration within base controller 
            IEnumerable<CategoryHome> parents = null;
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
                    builder.Add(S["Channels"], channels => channels
                        .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary()
                        {
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], channel => channel
                            .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
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

            // Get all categories & return views
            var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<CategoryListViewModel<CategoryHome>>("Article.Categories.Display.Sidebar", model =>
                {
                    model.Categories = categories?.Where(c => c.Id == entity.CategoryId);
                    return model;
                }).Zone("sidebar").Order(2)
            );

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
            IEnumerable<CategoryHome> parents = null;
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
                            ["opts.id"] = "",
                            ["opts.alias"] = ""
                        })
                        .LocalNav()
                    );
                    foreach (var parent in parents)
                    {
                        builder.Add(S[parent.Name], category => category
                            .Action("Index", "Home", "Plato.Articles.Categories", new RouteValueDictionary
                            {
                                ["opts.id"] = parent.Id,
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
           
                builder.Add(S[entity.Id > 0 ? "Edit Comment" : "New Comment"]);

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
                View<CategoryDropDownViewModel>("Article.Categories.Edit.Sidebar", model => viewModel).Zone("sidebar").Order(1)
            );

        }
        
        public override async Task<bool> ValidateModelAsync(Article entity, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new CategoryInputViewModel
            {
                SelectedCategories = GetCategoriesToAdd()
            });

        }

        public override async Task ComposeTypeAsync(Article entity, IUpdateModel updater)
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

                    // Build channels to remove
                    var categoriesToRemove = new List<int>();
                    foreach (var channel in await GetCategoryIdsByEntityIdAsync(article))
                    {
                        if (!categoriesToAdd.Contains(channel))
                        {
                            categoriesToRemove.Add(channel);
                        }
                    }

                    // Remove categories
                    foreach (var channelId in categoriesToRemove)
                    {
                        await _entityCategoryStore.DeleteByEntityIdAndCategoryId(article.Id, channelId);
                    }
                    
                    // Get current user
                    var user = await _contextFacade.GetAuthenticatedUserAsync();

                    // Add new entity category relationship
                    foreach (var channelId in categoriesToAdd)
                    {
                        await _entityCategoryStore.CreateAsync(new EntityCategory()
                        {
                            EntityId = article.Id,
                            CategoryId = channelId,
                            CreatedUserId = user?.Id ?? 0,
                            ModifiedUserId = user?.Id ?? 0,
                        });
                    }

                    // Update primary category
                    foreach (var channelId in categoriesToAdd)
                    {
                        if (channelId > 0)
                        {
                            article.CategoryId = channelId;
                            await _entityStore.UpdateAsync(article);
                            break;
                        }

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

            var categories = await _entityCategoryStore.GetByEntityId(entity.Id);;
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
