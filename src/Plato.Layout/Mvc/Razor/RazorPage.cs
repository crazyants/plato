using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Layout;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Plato.Layout.Elements;
using Plato.Layout.Display;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Layout.Mvc.Razor
{
    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        
        private ILayoutManager _layoutManager;
        public ILayoutManager LayoutManager
        {
            get
            {
                EnsureLayoutManager();
                return _layoutManager;
            }
        }

        private void EnsureLayoutManager()
        {
            if (_layoutManager == null)
            {
                _layoutManager = ViewContext.HttpContext.RequestServices.GetService<ILayoutManager>();
                _layoutManager.ViewContext = this.ViewContext;
            }
        }

        public IHtmlContent Display(string Shape, dynamic model)
        {
            return LayoutManager.Display(Shape, model);        
        }

        public async Task<IHtmlContent> DisplayAsync(string Shape, dynamic model)
        {
            return await LayoutManager.DisplayAsync(Shape, model);
        }

        private IViewLocalizer _t;

        public IViewLocalizer T
        {
            get
            {
                if (_t == null)
                {
                    _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware)_t).Contextualize(this.ViewContext);
                }

                return _t;
            }
        }

        //public IHtmlContent BuildElement(string fullName)
        //{

        //    var element = new Element()
        //        .Add("Plato.Modules.Login");

        //    var helper = new DisplayHelper();

        //    Task<IHtmlContent> task = helper.ShapeExecuteAsync(element);
        //    task.Wait();
        //    return task.Result;

        //}

        //public IHtmlContent Zone(dynamic Display, dynamic Shape)
        //{
        //    var htmlContents = new List<IHtmlContent>();
        //    foreach (var item in Shape)
        //    {
        //        htmlContents.Add(Display(item));
        //    }

        //    var htmlContentBuilder = new HtmlContentBuilder();
        //    foreach (var htmlContent in htmlContents)
        //    {
        //        htmlContentBuilder.AppendHtml(htmlContent);
        //    }

        //    return htmlContentBuilder;
        //}


        //public IHtmlContent ContentZone(dynamic Display, dynamic Shape)
        //{

        //    var htmlContents = new List<IHtmlContent>();

        //    var shapes = ((IEnumerable<dynamic>)Shape);
        //    var tabbed = shapes.GroupBy(x => (string)x.Metadata.Tab).ToList();

        //    if (tabbed.Count > 1)
        //    {
        //        foreach (var tab in tabbed)
        //        {
        //            var tabName = string.IsNullOrWhiteSpace(tab.Key) ? "Content" : tab.Key;
        //            var tabBuilder = new TagBuilder("div");
        //            tabBuilder.Attributes["id"] = "tab-" + tabName;
        //            tabBuilder.Attributes["data-tab"] = tabName;
        //            foreach (var item in tab)
        //            {
        //                tabBuilder.InnerHtml.AppendHtml(Display(item));
        //            }
        //            htmlContents.Add(tabBuilder);
        //        }
        //    }
        //    else if (tabbed.Count > 0)
        //    {
        //        foreach (var item in tabbed[0])
        //        {
        //            htmlContents.Add(Display(item));
        //        }
        //    }

        //    var htmlContentBuilder = new HtmlContentBuilder();
        //    foreach (var htmlContent in htmlContents)
        //    {
        //        htmlContentBuilder.AppendHtml(htmlContent);
        //    }

        //    return htmlContentBuilder;
        //}

        //public new Task<IHtmlContent> RenderSectionAsync(string name, bool required)
        //{

        //    //IHtmlContent result = Display(zone);

        //    var htmlContentBuilder = new HtmlContentBuilder();
        //    htmlContentBuilder.AppendHtml("<div>zone 1</div>");

        //    return Task.FromResult((IHtmlContent)htmlContentBuilder);

        //}

    }

    public abstract class RazorPage : RazorPage<dynamic>
    {

    }

}
