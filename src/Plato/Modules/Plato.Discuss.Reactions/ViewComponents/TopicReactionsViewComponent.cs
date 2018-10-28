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

        private readonly IReactionsEntryStore<ReactionEntry> _reactionsEntryStore;

        public TopicReactionsViewComponent(
            IReactionsEntryStore<ReactionEntry> reactionsEntryStore)
        {
            _reactionsEntryStore = reactionsEntryStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {

            return View(new TopicReactionsViewModel()
            {
                Topic = topic,
                Reply = reply,
                GroupedReactions = await _reactionsEntryStore.GetReactionsGroupedAsync(topic.Id, reply?.Id ?? 0)
            });

        }

    }

}
