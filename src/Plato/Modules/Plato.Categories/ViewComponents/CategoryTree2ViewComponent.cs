using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.ViewModels;

namespace Plato.Categories.ViewComponents
{

    public class CategoryTree2ViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(CategoryTreeViewModel model)
        {

            if (model == null)
            {
                model = new CategoryTreeViewModel();
            }
            
            return Task.FromResult((IViewComponentResult) View(model));

        }

    }

}
