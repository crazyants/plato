using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ViewProviders;
using Plato.Articles.Models;
using Plato.Articles.Drafts.ViewModels;
using Plato.Entities.Stores;

namespace Plato.Articles.Drafts.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        public const string HtmlName = "published";
        
        private readonly HttpRequest _request;
        private readonly IEntityStore<Article> _entityStore;

        public ArticleViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Article> entityStore)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article article, IViewProviderContext updater)
        {

            // Ensure entity exists before attempting to update
            Article entity = null;
            if (article.Id > 0)
            {
                entity = await _entityStore.GetByIdAsync(article.Id);
            }
            
            return Views(
                View<DraftViewModel>("Article.Draft.Edit.Sidebar", model =>
                {
                    model.HtmlName = HtmlName;
                    model.Published = !entity?.IsPrivate ?? false;
                    return model;
                }).Zone("sidebar").Order(10)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext updater)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(article.Id);
            if (entity == null)
            {
                return await BuildEditAsync(article, updater);
            }
            
            // Get the checkbox value
            var published = false;
            foreach (var key in _request.Form.Keys)
            {
                if (key == HtmlName)
                {
                    var values = _request.Form[key];
                    if (!String.IsNullOrEmpty(values))
                    {
                        published = true;
                        break;
                    }
                }
            }
            
            // Change private flag
            entity.IsPrivate = !published;

            // Update entity
            await _entityStore.UpdateAsync(entity);

            // Return
            return await BuildEditAsync(article, updater);

        }

    }

}
