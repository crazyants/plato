using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Ideas.Categories.Models;

namespace Plato.Ideas.Categories.ViewComponents
{
    
    public class EditIdeaCategoryMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<Category> _categoryStore;
       
        public EditIdeaCategoryMenuViewComponent(
            ICategoryStore<Category> categoryStore)
        {
            _categoryStore = categoryStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId)
        {
            var category = await _categoryStore.GetByIdAsync(categoryId);
            return View(category);
        }
        
    }

}
