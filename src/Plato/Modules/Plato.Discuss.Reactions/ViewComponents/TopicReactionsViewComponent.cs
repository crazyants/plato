using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Reactions.ViewModels;
using Plato.Entities.Stores;
using Plato.Reactions.Models;
using Plato.Reactions.Services;
using Plato.Reactions.Stores;

namespace Plato.Discuss.Reactions.ViewComponents
{

    public class TopicReactionsViewComponent : ViewComponent
    {

        private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IReactionsManager<Reaction> _reactionManager;

        public TopicReactionsViewComponent(
            IEntityStore<Topic> entityStore,
            IReactionsManager<Reaction> reactionManager,
            IEntityReactionsStore<EntityReaction> entityReactionsStore)
        {
            _entityStore = entityStore;
            _reactionManager = reactionManager;
            _entityReactionsStore = entityReactionsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {

            var viewModel = new TopicReactionsViewModel()
            {
                Topic = await _entityStore.GetByIdAsync(id),
                Reactions = await GetReactions(id)
            };

            return View(viewModel);
        }

        private async Task<IDictionary<string, IList<IReaction>>> GetReactions(int id)
        {

            var output = new ConcurrentDictionary<string, IList<IReaction>>();
            var reactions = await _entityReactionsStore.GetEntityReactionsAsync(id);
            if (reactions != null)
            {
                foreach (var reaction in reactions)
                {
                    output.AddOrUpdate(reaction.Emoji, new List<IReaction>()
                    {
                        reaction
                    }, (k, v) =>
                    {
                        v.Add(reaction);
                        return v;
                    });
                }
             
            }

            return output;
        }

    }

}
