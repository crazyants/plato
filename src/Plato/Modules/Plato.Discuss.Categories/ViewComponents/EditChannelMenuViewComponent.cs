using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Discuss.Categories.Models;

namespace Plato.Discuss.Categories.ViewComponents
{
    
    public class EditChannelMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<Category> _channelStore;
       
        public EditChannelMenuViewComponent(
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
