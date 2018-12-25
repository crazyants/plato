using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{

    public class GetTopicReplyListViewComponent : ViewComponent
    {
        
        private readonly IReplyService _replyService;

        public GetTopicReplyListViewComponent(
            IReplyService replyService)
        {
            _replyService = replyService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TopicOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TopicOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<TopicViewModel> GetViewModel(
            TopicOptions options,
            PagerOptions pager)
        {
            
            var results = await _replyService.GetRepliesAsync(options, pager);
            
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TopicViewModel
            {
                Options = options,
                Pager = pager,
                Replies = results
        };

        }

    }

}
