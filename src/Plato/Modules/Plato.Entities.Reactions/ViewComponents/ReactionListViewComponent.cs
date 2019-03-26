using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Reactions.ViewModels;
using Plato.Entities.Reactions.Stores;

namespace Plato.Entities.Reactions.ViewComponents
{

    public class ReactionListViewComponent : ViewComponent
    {

        private readonly ISimpleReactionsStore _simpleReactionsStore;

        public ReactionListViewComponent(
            ISimpleReactionsStore simpleReactionsStore)
        {
            _simpleReactionsStore = simpleReactionsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReactionListViewModel model)
        {

            // We always need an entity
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Entity == null)
            {
                throw new ArgumentNullException(nameof(model.Entity));
            }
            
            model.Reactions =
                await _simpleReactionsStore.GetSimpleReactionsAsync(model.Entity.Id, model.Reply?.Id ?? 0);

            // Return view
            return View(model);

        }

    }

}
