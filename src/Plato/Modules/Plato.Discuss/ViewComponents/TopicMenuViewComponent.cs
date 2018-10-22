using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.Stores;

namespace Plato.Discuss.ViewComponents
{
    public class TopicMenuViewComponent : ViewComponent
    {

     
        private readonly IEntityStore<Topic> _entityStore;

        public TopicMenuViewComponent(IEntityStore<Topic> entityStore)
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
