using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Stores;

namespace Plato.Discuss.Reactions.ViewComponents
{
  
    public class ReactMenuViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;

        public ReactMenuViewComponent(IEntityStore<Topic> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var model = await _entityStore.GetByIdAsync(id);
            return View(model);
        }

    }


}
