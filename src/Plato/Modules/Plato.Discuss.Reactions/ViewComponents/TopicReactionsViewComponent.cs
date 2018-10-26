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

        private readonly IReactionsStore<Reaction> _reactionsStore;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IReactionsManager<Reaction> _reactionManager;

        public TopicReactionsViewComponent(
            IEntityStore<Topic> entityStore,
            IReactionsManager<Reaction> reactionManager,
            IReactionsStore<Reaction> reactionsStore)
        {
            _entityStore = entityStore;
            _reactionManager = reactionManager;
            _reactionsStore = reactionsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {

            var reactions = await _reactionsStore.GetEntityReactionsGroupedByEmojiAsync(topic.Id);

            if (reply != null)
            {

            }

            var viewModel = new TopicReactionsViewModel()
            {
                Topic = topic,
                Reply = reply,
                Reactions = reactions
            };

            return View(viewModel);
        }


    }

}
