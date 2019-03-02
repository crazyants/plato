using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Discuss.ViewComponents
{

    public class TopicReplyListViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        private readonly IEntityReplyService<Reply> _replyService;

        public TopicReplyListViewComponent(
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityStore<Topic> entityStore,
            IEntityReplyService<Reply> replyService)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _replyService = replyService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            EntityOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<EntityViewModel<Topic, Reply>> GetViewModel(
            EntityOptions options,
            PagerOptions pager)
        {

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService.GetRepliesAsync(options, pager);
            
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityViewModel<Topic, Reply>
            {
                Options = options,
                Pager = pager,
                Entity = entity,
                Replies = results
        };

        }

    }

}
