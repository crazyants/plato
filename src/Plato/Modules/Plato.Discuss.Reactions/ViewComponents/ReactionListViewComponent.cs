using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Reactions.ViewModels;
using Plato.Reactions.Models;
using Plato.Reactions.Stores;

namespace Plato.Discuss.Reactions.ViewComponents
{

    public class ReactionListViewComponent : ViewComponent
    {

        private readonly ISimpleReactionsStore<ReactionEntry> _simpleReactionsStore;

        public ReactionListViewComponent(
            ISimpleReactionsStore<ReactionEntry> simpleReactionsStore)
        {
            _simpleReactionsStore = simpleReactionsStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {

            return View(new ReactionListViewModel()
            {
                Topic = topic,
                Reply = reply,
                Reactions = await _simpleReactionsStore.GetSimpleReactionsAsync(topic.Id, reply?.Id ?? 0)
            });

        }

    }

}
