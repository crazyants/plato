using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListViewComponent : ViewComponent
    {

        private readonly ICategoryService<Category> _categoryService;

        public ArticleCategoryListViewComponent(ICategoryService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }
            
            return View(await GetIndexModel(options));

        }
        
        async Task<CategoryListViewModel<Category>> GetIndexModel(CategoryIndexOptions options)
        {
            
            // Get categories
            var categories = await _categoryService
                .GetResultsAsync(options, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

            return new CategoryListViewModel<Category>()
            {
                Options = options,
                Categories = categories?.Data?.Where(c => c.ParentId == options.CategoryId) ?? new List<Category>()
            };

        }

    }
    
}
