using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Issues.Categories.Models;

namespace Plato.Issues.Categories.ViewComponents
{
    
    public class EditIssueCategoryMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<Category> _categoryStore;
       
        public EditIssueCategoryMenuViewComponent(
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
