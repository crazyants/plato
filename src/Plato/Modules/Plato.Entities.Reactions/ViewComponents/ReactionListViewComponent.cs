using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
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

        public async Task<IViewComponentResult> InvokeAsync(IEntity entity, IEntityReply reply)
        {

            // We always need an entity
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            // Return view
            return View(new ReactionListViewModel()
            {
                Entity = entity,
                Reply = reply,
                Reactions = await _simpleReactionsStore.GetSimpleReactionsAsync(entity.Id, reply?.Id ?? 0)
            });

        }

    }

}
