using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.ViewModels;
using Plato.Questions.Categories.Models;

namespace Plato.Questions.Categories.ViewComponents
{

    public class QuestionCategoryListItemViewComponent : ViewComponent
    {
 
        public QuestionCategoryListItemViewComponent()
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
