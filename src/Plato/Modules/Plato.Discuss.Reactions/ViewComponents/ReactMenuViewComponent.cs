using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Reactions.ViewModels;
using Plato.Entities.Stores;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Discuss.Reactions.ViewComponents
{
  
    public class ReactMenuViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IReactionsManager<Reaction> _reactionManager;

        public ReactMenuViewComponent(
            IEntityStore<Topic> entityStore,
            IReactionsManager<Reaction> reactionManager)
        {
            _entityStore = entityStore;
            _reactionManager = reactionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
    
            var viewModel = new ReactMenuViewModel()
            {
                Topic = await _entityStore.GetByIdAsync(id),
                Reactions = _reactionManager.GetReactions()
            };
           
            return View(viewModel);
        }

    }

}
