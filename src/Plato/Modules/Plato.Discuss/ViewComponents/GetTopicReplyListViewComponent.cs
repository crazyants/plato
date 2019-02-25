using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.ViewComponents
{

    public class GetTopicReplyListViewComponent : ViewComponent
    {
        
        private readonly IReplyService _replyService;
        private readonly IEntityStore<Topic> _entityStore;

        public GetTopicReplyListViewComponent(
            IReplyService replyService,
            IEntityStore<Topic> entityStore)
        {
            _replyService = replyService;
            _entityStore = entityStore;
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
            
          
            var topic = await _entityStore.GetByIdAsync(options.Params.EntityId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService.GetRepliesAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new TopicViewModel
            {
                Options = options,
                Pager = pager,
                Topic = topic,
                Replies = results
            };

        }

    }

}
