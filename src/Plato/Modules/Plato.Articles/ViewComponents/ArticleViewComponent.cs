using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;

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

        public async Task<IViewComponentResult> InvokeAsync(EntityOptions options)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            var model = await GetViewModel(options);

            return View(model);

        }

        async Task<EntityViewModel<Article, ArticleComment>> GetViewModel(
            EntityOptions options)
        {

            if (options.EntityId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(options.EntityId));
            }

            var topic = await _entityStore.GetByIdAsync(options.EntityId);
            if (topic == null)
            {
                throw new ArgumentNullException();
            }
            
            // Return view model
            return new EntityViewModel<Article, ArticleComment>
            {
                Options = options,
                Entity = topic
        };

        }

    }

}
