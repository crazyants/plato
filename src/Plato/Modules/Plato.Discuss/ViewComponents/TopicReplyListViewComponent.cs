using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{

    public class TopicReplyListViewComponent : ViewComponent
    {

        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;

        public TopicReplyListViewComponent(
            IEntityReplyStore<Reply> entityReplyStore,
            IEntityStore<Topic> entityStore)
        {
            _entityReplyStore = entityReplyStore;
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

            var model = await GetViewModel(options, pager);

            return View(model);

        }

        async Task<TopicViewModel> GetViewModel(
            TopicOptions options,
            PagerOptions pager)
        {

            if (options.Params.TopicId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Params.TopicId));
            }

            var topic = await _entityStore.GetByIdAsync(options.Params.TopicId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }
            
            // Get results
            var results =     await _entityReplyStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityReplyQueryParams>(q =>
                {
                    q.EntityId.Equals(topic.Id);
                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
                })
                .OrderBy("CreatedDate")
                .ToList();

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
