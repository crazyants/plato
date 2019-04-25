using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Titles;
using Plato.Internal.Layout.Views;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Layout.Razor
{
    
    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {

        private IViewLocalizer _t;
        private User _currentUser;
        private IViewDisplayHelper _viewDisplayHelper;
        private IPageTitleBuilder _pageTitleBuilder;
        private IContextFacade _contextFacade;

        public User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    // Attempt to get user set via our AuthenticatedUserMiddleware
                    var user = ViewContext.HttpContext.Features[typeof(User)];
                    _currentUser = (User) user;
                }

                return _currentUser;
            }
        }

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

    
        public IPageTitleBuilder Title =>
            _pageTitleBuilder ??
            (_pageTitleBuilder = Context.RequestServices.GetRequiredService<IPageTitleBuilder>());

        public IContextFacade ContextFacade =>
            _contextFacade ??
            (_contextFacade = Context.RequestServices.GetRequiredService<IContextFacade>());


        public void AddTitleSegment(IHtmlContent segment, int order = 0)
        {
            Title.AddSegment(segment, order);
        }

        public IHtmlContent RenderTitleSegments(string segment, int order = 0, IHtmlContent separator = null)
        {
            if (!String.IsNullOrEmpty(segment))
            {
                Title.AddSegment(new HtmlString(HtmlEncoder.Encode(segment)), order);
            }

            return Title.GenerateTitle(separator);
        }
        

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
                var htmlContentBuilder = builder.AppendHtml(viewResult);
            }

            return builder;
        }

        public string GetRouteUrl(RouteValueDictionary routeValues)
        {
            var facade = Context.RequestServices.GetService<IContextFacade>();
            return facade.GetRouteUrl(routeValues);
        }

    }
    
}
