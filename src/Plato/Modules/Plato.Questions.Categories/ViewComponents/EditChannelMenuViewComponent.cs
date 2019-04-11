using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Questions.Categories.Models;

namespace Plato.Questions.Categories.ViewComponents
{
    
    public class EditQuestionCategoryMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<Category> _categoryStore;
       
        public EditQuestionCategoryMenuViewComponent(
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
