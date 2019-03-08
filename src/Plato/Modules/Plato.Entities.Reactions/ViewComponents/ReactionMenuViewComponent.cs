using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Reactions.Models;
using Plato.Reactions.Services;

namespace Plato.Entities.Reactions.ViewComponents
{
  
    public class ReactionMenuViewComponent : ViewComponent
    {
  
        private readonly IReactionsManager<Reaction> _reactionManager;

        public ReactionMenuViewComponent(
            IReactionsManager<Reaction> reactionManager)
        {
            _reactionManager = reactionManager;
        }

        public Task<IViewComponentResult> InvokeAsync(IEntity entity, IEntityReply reply)
        {
    
            var viewModel = new ReactionMenuViewModel()
            {
                Entity = entity,
                Reply = reply,
                Reactions = _reactionManager.GetReactions()
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));
        }

    }

}
