using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Stores;

namespace Plato.Docs.ViewComponents
{
    
    public class DocTreeMenuViewComponent : ViewComponent
    {

        private readonly IEntityStore<Doc> _entityStore;
       
        public DocTreeMenuViewComponent(IEntityStore<Doc> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int entityId)
        {

            if (entityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(entityId));
            }

            return View(await _entityStore.GetByIdAsync(entityId));

        }

    }

}
