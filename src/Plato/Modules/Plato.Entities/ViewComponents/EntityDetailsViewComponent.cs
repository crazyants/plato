using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;

namespace Plato.Entities.ViewComponents
{

    public class EntityDetailsViewComponent : ViewComponent
    {

        private readonly IEntityStore<Entity> _entityStore;

        public EntityDetailsViewComponent(
            IEntityStore<Entity> entityStore)
        {
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {

            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            
            return View(await GetViewModel(id));

        }

        async Task<EntityDetailsViewModel> GetViewModel(int id)
        {
            
            var entity = await _entityStore.GetByIdAsync(id);
            return new EntityDetailsViewModel()
            {
                Entity = entity
            };

        }

    }

}
