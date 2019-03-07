using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Categories.Models;
using Plato.Articles.Categories.ViewModels;
using Plato.Categories.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListItemViewComponent : ViewComponent
    {
 
        public ArticleCategoryListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Category category,  CategoryIndexOptions options)
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
