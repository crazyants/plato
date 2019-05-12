using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Articles.Categories.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListSideBar : ViewComponent
    {

        private readonly ICategoryService<Category> _categoryService;
        
        public ArticleCategoryListSideBar(ICategoryService<Category> categoryService)
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
                .GetResultsAsync(new CategoryIndexOptions()
                {
                    FeatureId = options.FeatureId,
                    CategoryId = 0
                }, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });
            
            return new CategoryListViewModel<Category>()
            {
                Options = options,
                Categories = categories?.Data?.Where(c => c.ParentId == 0)
            };
        }


    }


}
