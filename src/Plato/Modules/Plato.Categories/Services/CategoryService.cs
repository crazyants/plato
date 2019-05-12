using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Categories.Services
{
   
    public class CategoryService<TModel> : ICategoryService<TModel> where TModel : class, ICategory
    {

        private Action<QueryOptions> _configureDb = null;
        private Action<CategoryQueryParams> _configureParams = null;

        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<TModel> _categoryStore;

        public CategoryService(
            IContextFacade contextFacade,
            ICategoryStore<TModel> categoryStore)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;

            // Default options delegate
            _configureDb = options => options.SearchType = SearchTypes.Tsql;

        }

        public ICategoryService<TModel> ConfigureDb(Action<IQueryOptions> configure)
        {
            _configureDb = configure;
            return this;
        }

        public ICategoryService<TModel> ConfigureQuery(Action<CategoryQueryParams> configure)
        {
            _configureParams = configure;
            return this;
        }

        public async Task<IPagedResults<TModel>> GetResultsAsync(CategoryIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Ensure we have a sort column is non is specified
            if (options.Sort == SortBy.Auto)
            {
                options.Sort = SortBy.SortOrder;
                options.Order = OrderBy.Asc;
            }

            // Get authenticated user 
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Return tailored results
            return await _categoryStore.QueryAsync()
                .Take(pager.Page, pager.Size)
                .Configure(_configureDb)
                .Select<CategoryQueryParams>(q =>
                {

                    // ----------------
                    // Set current authenticated user id
                    // This is required for various security checks
                    // ----------------

                    q.UserId.Equals(user?.Id ?? 0);

                    // ----------------
                    // Basic parameters
                    // ----------------


                    // CategoryId
                    if (options.CategoryId > 0)
                    {
                        q.FeatureId.Equals(options.CategoryId);
                    }

                    // FeatureId
                    if (options.FeatureId > 0)
                    {
                        q.FeatureId.Equals(options.FeatureId);
                    }

                    // ----------------
                    // Additional parameter configuration
                    // ----------------

                    _configureParams?.Invoke(q);

                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}
