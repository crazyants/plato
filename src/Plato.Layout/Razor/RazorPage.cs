using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Layout.Adaptors;
using Plato.Layout.TagHelpers;
using Plato.Layout.Views;

namespace Plato.Layout.Razor
{




    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {

        private IViewLocalizer _t;

        public IViewLocalizer T
        {
            get
            {
                if (_t == null)
                {
                    _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware) _t).Contextualize(this.ViewContext);
                }

                return _t;
            }
        }

        private IViewDisplayHelper _viewDisplayHelper;

        private void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                var factory = Context.RequestServices.GetService<IViewHelperFactory>();
                _viewDisplayHelper = factory.CreateHelper(ViewContext);
            }
        }

        public async Task<IHtmlContent> DisplayAsync(IGenericView view)
        {
            EnsureViewHelper();
            return await _viewDisplayHelper.DisplayAsync(view);
        }

        public async Task<IHtmlContent> DisplayAsync(IEnumerable<IGenericView> views)
        {

            var builder = new HtmlContentBuilder();
            foreach (var view in views)
            {
                var viewResult = await DisplayAsync(view);
                builder.AppendHtml(viewResult);
            }

            return builder;
        }

    }


}
