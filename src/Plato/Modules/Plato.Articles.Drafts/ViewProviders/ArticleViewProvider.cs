using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Layout.ViewProviders;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Drafts.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        public static string HtmlName = "published";
        
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

        public override Task<IViewProviderResult> BuildEditAsync(Article article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task ComposeTypeAsync(Article question, IUpdateModel updater)
        {

            var model = new SelectDropDownViewModel()
            {
                SelectedValue = GetIsPrivate().ToString()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                question.IsPrivate = GetIsPrivate();
                question.IsHidden = GetIsHidden();
            }

        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(Article model, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return await BuildEditAsync(model, context);
            }

            // Validate model
            if (await ValidateModelAsync(model, context.Updater))
            {
                if (!model.IsNew)
                {
                    model.IsPrivate = GetIsPrivate();
                    model.IsHidden = GetIsHidden();
                    await _entityStore.UpdateAsync(model);
                }

            }

            return await BuildEditAsync(model, context);


        }

        bool GetIsPrivate()
        {
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (value.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        bool GetIsHidden()
        {
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (value.IndexOf("hidden", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

    }

}
