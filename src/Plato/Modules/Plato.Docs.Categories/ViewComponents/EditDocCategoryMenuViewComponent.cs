using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Docs.Categories.Models;

namespace Plato.Docs.Categories.ViewComponents
{
    
    public class EditDocCategoryMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<Category> _channelStore;
       
        public EditDocCategoryMenuViewComponent(
            ICategoryStore<Category> channelStore)
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
