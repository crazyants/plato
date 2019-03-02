using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.ViewComponents
{

    public class GetArticleCommentListViewComponent : ViewComponent
    {
        
        private readonly IEntityReplyService<Comment> _replyService;
        private readonly IEntityStore<Article> _entityStore;

        public GetArticleCommentListViewComponent(
            IEntityReplyService<Comment> replyService,
            IEntityStore<Article> entityStore)
        {
            _replyService = replyService;
            _entityStore = entityStore;
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

        async Task<EntityViewModel<Article, Comment>> GetViewModel(
            EntityOptions options,
            PagerOptions pager)
        {
            
          
            var topic = await _entityStore.GetByIdAsync(options.Id);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService.GetRepliesAsync(options, pager);

            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityViewModel<Article, Comment>
            {
                Options = options,
                Pager = pager,
                Entity = topic,
                Replies = results
            };

        }

    }

}
