using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Reactions.ViewModels;
using Plato.Reactions.Models;
using Plato.Reactions.Stores;

namespace Plato.Discuss.Reactions.ViewComponents
{

    public class TopicReactionsViewComponent : ViewComponent
    {

        private readonly IReactionsStore<Reaction> _reactionsStore;

        public TopicReactionsViewComponent(
            IReactionsStore<Reaction> reactionsStore)
        {
            _reactionsStore = reactionsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {

            return View(new TopicReactionsViewModel()
            {
                Topic = topic,
                Reply = reply,
                GroupedReactions = await _reactionsStore.GetEntityReactionsGroupedAsync(topic.Id, reply?.Id ?? 0)
            });

        }

    }

}
