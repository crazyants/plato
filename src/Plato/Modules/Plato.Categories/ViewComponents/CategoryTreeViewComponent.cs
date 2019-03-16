using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;

namespace Plato.Categories.ViewComponents
{
    public class CategoryTreeViewComponent : ViewComponent
    {

        private readonly ICategoryStore<CategoryBase> _categoryStore;
        
        public CategoryTreeViewComponent(
            ICategoryStore<CategoryBase> categoryStore)
        {
            _categoryStore = categoryStore;
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
                CssClass = options.CssClass
            });

        }

        private async Task<IList<Selection<CategoryBase>>> BuildSelectionsAsync(CategoryTreeOptions options)
        {
            
            var channels = await _categoryStore.GetByFeatureIdAsync(options.IndexOptions.FeatureId);

            return channels?.Select(c => new Selection<CategoryBase>
                {
                    IsSelected = options.SelectedCategories.Any(v => v == c.Id),
                    Value = c
                })
                .ToList();

        }
    }

}
