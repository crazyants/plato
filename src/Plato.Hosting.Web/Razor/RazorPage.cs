using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Layout;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Hosting.Web.Razor
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
            }
        }
                
        public async Task<IHtmlContent> DisplayAsync(string sectionName)
        {
            return await LayoutManager.DisplayAsync(sectionName);
        }


        public new Task<IHtmlContent> RenderSectionAsync(string name, bool required)
        {

            //IHtmlContent result = Display(zone);

            var htmlContentBuilder = new HtmlContentBuilder();
            htmlContentBuilder.AppendHtml("<div>zone 1</div>");

            return Task.FromResult((IHtmlContent)htmlContentBuilder);

        }

    }

    public abstract class RazorPage : RazorPage<dynamic>
    {

    }

}
