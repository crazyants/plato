using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        private IViewHelper _viewHelper;

        private void EnsureViewHelper()
        {
            if (_viewHelper == null)
            {
                var factory = Context.RequestServices.GetService<IViewHelperFactory>();
                _viewHelper = factory.CreateHelper(ViewContext);
            }
        }

        public Task<IHtmlContent> DisplayAsync(dynamic shape)
        {
            EnsureViewHelper();
            return _viewHelper.DisplayAsync(shape);
        }



    }


}
