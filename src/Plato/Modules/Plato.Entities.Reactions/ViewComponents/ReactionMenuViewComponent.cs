using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Reactions.ViewModels;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;

namespace Plato.Entities.Reactions.ViewComponents
{
  
    public class ReactionMenuViewComponent : ViewComponent
    {
  
        private readonly IReactionsManager<Reaction> _reactionManager;

        public ReactionMenuViewComponent(IReactionsManager<Reaction> reactionManager)
        {
            _reactionManager = reactionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReactionMenuViewModel model)
        {

            // Attempt to load reactions for supplied feature Id
            var reactions = await _reactionManager.GetCategorizedReactionsAsync();
            if (reactions.ContainsKey(model.ModuleId))
            {
                model.Reactions = reactions[model.ModuleId];
            }

            // Else fall back to default reactions
            if (model.Reactions == null)
            {
                model.Reactions = DefaultReactions.GetReactions();
            }

            return View(model);
        }

    }

}
