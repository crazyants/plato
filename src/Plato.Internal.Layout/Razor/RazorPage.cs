using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Internal.Layout.Views;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Internal.Layout.Razor
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

        public async Task<IHtmlContent> DisplayAsync(IView view)
        {
            EnsureViewHelper();
            return await _viewDisplayHelper.DisplayAsync(view);
        }

        public async Task<IHtmlContent> DisplayAsync(IEnumerable<IView> views)
        {

            var builder = new HtmlContentBuilder();
            foreach (var view in views)
            {
                var viewResult = await DisplayAsync(view);
                builder.AppendHtml(viewResult);
            }

            return builder;
        }
        
        public async Task<User> GetAuthenticatedUserAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = ViewContext.HttpContext.RequestServices.GetRequiredService<IPlatoUserStore<User>>();
                return await userStore.GetByUserNameAsync(User.Identity.Name);
            }

            return null;
        }



    }


}
