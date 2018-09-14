using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;

namespace Plato.Discuss.Channels.ViewComponents
{


    public class EditChannelMenuViewComponent : ViewComponent
    {

        private readonly ICategoryStore<CategoryBase> _channelStore;
       
        public EditChannelMenuViewComponent(
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
