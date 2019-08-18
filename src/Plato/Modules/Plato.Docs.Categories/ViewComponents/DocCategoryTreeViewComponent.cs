using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.ViewModels;
using Plato.Docs.Categories.Models;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Categories.ViewComponents
{

    public class DocCategoryTreeViewComponent : ViewComponent
    {

        private readonly ICategoryService<Category> _categoryService;
     
        public DocCategoryTreeViewComponent(ICategoryService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryTreeOptions options)
        {

            if (options == null)
            {
                options = new CategoryTreeOptions();
            }

            if (options.SelectedCategories == null)
            {
                options.SelectedCategories = new int[0];
            }

            return View(new CategoryTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedCategories = await BuildCategories(options),
                CssClass = options.CssClass,
                RouteValues = options.RouteValues
            });

        }
        

        private async Task<IList<Selection<CategoryBase>>> BuildCategories(CategoryTreeOptions options)
        {

            // Get categories
            var categories = await _categoryService
                .GetResultsAsync(options.IndexOptions, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

            return categories?.Data.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }

    }
    
}
