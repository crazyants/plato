using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.ViewModels;
using Plato.Issues.Categories.Models;

namespace Plato.Issues.Categories.ViewComponents
{

    public class IssueCategoryListItemViewComponent : ViewComponent
    {
 
        public IssueCategoryListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Category category, CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }

            var model = new CategoryListItemViewModel<Category>()
            {
                Category = category,
                Options = options
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }
        
    }
    
}
