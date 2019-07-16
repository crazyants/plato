using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Categories.ViewComponents
{
    public class CategoryTreeViewComponent : ViewComponent
    {

        private readonly ICategoryService<CategoryBase> _categoryService;

        public CategoryTreeViewComponent(ICategoryService<CategoryBase> categoryService)
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

            var selected = await BuildSelectionsAsync(options);
            return View(new CategoryTreeViewModel
            {
                HtmlName = options.HtmlName,
                EnableCheckBoxes = options.EnableCheckBoxes,
                EditMenuViewName = options.EditMenuViewName,
                SelectedCategories = selected,
                CssClass = options.CssClass,
                RouteValues = options.RouteValues
            });

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(CategoryTreeOptions options)
        {
            
            // Get categories
            var categories = await _categoryService.GetResultsAsync(
                options.IndexOptions, new PagerOptions()
                {
                    Page = 1,
                    Size = int.MaxValue
                });

            return categories?.Data?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }
    }

}
