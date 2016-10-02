using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Layout
{
    public class LayoutManager : ILayoutManager
    {

        IViewComponentHelper _viewComponentHelper;
        public LayoutManager(
            IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }
        
        public Task<IHtmlContent> DisplayAsync(string sectionName)
        {
         
            var ctl = _viewComponentHelper.InvokeAsync("Plato.Modules.Template", new { value = "test" });

            var htmlContentBuilder = new HtmlContentBuilder();
            htmlContentBuilder.AppendHtml("<div>zone 1</div>");

            return Task.FromResult((IHtmlContent)htmlContentBuilder);

        }


    }
}
