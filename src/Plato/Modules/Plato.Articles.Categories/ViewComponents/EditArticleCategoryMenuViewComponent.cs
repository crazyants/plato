using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;

namespace Plato.Articles.Categories.ViewComponents
{
    
    public class EditArticleCategoryMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<CategoryBase> _channelStore;
       
        public EditArticleCategoryMenuViewComponent(
            ICategoryStore<CategoryBase> channelStore)
        {
            _channelStore = channelStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int categoryId)
        {
            var category = await _channelStore.GetByIdAsync(categoryId);
            return View(category);
        }


    }
}
