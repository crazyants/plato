using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Articles.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Navigation;

namespace Plato.Articles.ViewComponents
{

    public class ArticleViewComponent : ViewComponent
    {

        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<ArticleComment> _entityReplyStore;

        public ArticleViewComponent(
            IEntityReplyStore<ArticleComment> entityReplyStore,
            IEntityStore<Article> entityStore)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(TopicOptions options)
        {

            if (options == null)
            {
                options = new TopicOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<ArticleViewModel> GetViewModel(
            TopicOptions options)
        {

            if (options.Params.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.Params.EntityId));
            }

            var topic = await _entityStore.GetByIdAsync(options.Params.EntityId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }
            
            // Return view model
            return new ArticleViewModel
            {
                Options = options,
                Article = topic
        };

        }

    }

}
