using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Articles.Services;
using Plato.Articles.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Abstractions.Extensions;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;

namespace Plato.Articles.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        private const string EditorHtmlName = "message";

 
        private readonly IEntityStore<Article> _entityStore;
        private readonly IEntityReplyStore<ArticleComment> _entityReplyStore;
        private readonly IPostManager<Article> _articleManager;
  
        private readonly HttpRequest _request;
        
        public ArticleViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityReplyStore<ArticleComment> entityReplyStore,
            IEntityStore<Article> entityStore,
            IPostManager<Article> articleManager)
        {
    
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _articleManager = articleManager;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Article article, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Entity>)] as EntityIndexViewModel<Article>;
            
            return Task.FromResult(Views(
                View<EntityIndexViewModel<Article>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Article>>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel<Article>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Article article, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Article, ArticleComment>)] as EntityViewModel<Article, ArticleComment>;
            
            // Increment entity view count
            await IncrementTopicViewCount(article);
        
            return Views(
                View<Article>("Home.Entity.Header", model => article).Zone("header"),
                View<Article>("Home.Entity.Tools", model => article).Zone("tools"),
                View<Article>("Home.Entity.Sidebar", model => article).Zone("sidebar"),
                View<EntityViewModel<Article, ArticleComment>>("Home.Entity.Content", model => viewModel).Zone("content"),
                View<EditReplyViewModel>("Home.Entity.Footer", model => new EditReplyViewModel()
                {
                    EntityId = article.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer")
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Article article, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = article.Message;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }
            }
          
            var viewModel = new EditTopicViewModel()
            {
                Id = article.Id,
                Title = article.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = article.Alias
            };
     
            return Task.FromResult(Views(
                View<EditTopicViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditTopicViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditTopicViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Article article, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditTopicViewModel
            {
                Title = article.Title,
                Message = article.Message
            });
        }

        public override async Task ComposeTypeAsync(Article article, IUpdateModel updater)
        {

            var model = new EditTopicViewModel
            {
                Title = article.Title,
                Message = article.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                article.Title = model.Title;
                article.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Article article, IViewProviderContext context)
        {
            
            if (article.IsNewTopic)
            {
                return default(IViewProviderResult);
            }

            var entity = await _entityStore.GetByIdAsync(article.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(article, context);
            }
            
            // Validate 
            if (await ValidateModelAsync(article, context.Updater))
            {
                // Update
                var result = await _articleManager.UpdateAsync(article);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(article, context);

        }

        #endregion
        
        #region "Private Methods"
        
        async Task IncrementTopicViewCount(Article article)
        {
            article.TotalViews = article.TotalViews + 1;
            article.DailyViews = article.TotalViews.ToSafeDevision(DateTimeOffset.Now.DayDifference(article.CreatedDate));
            await _entityStore.UpdateAsync(article);
        }

        #endregion

    }

}
