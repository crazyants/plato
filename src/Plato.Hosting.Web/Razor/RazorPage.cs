using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace Plato.Hosting.Web.Razor
{
    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {


        //public IHtmlContent Display(string shape)
        //{
        //    var htmlContentBuilder = new HtmlContentBuilder();
        //    htmlContentBuilder.AppendHtml("<div>zone 1</div>");

        //    return (IHtmlContent)htmlContentBuilder;

        //}

        public new Task<IHtmlContent>  RenderSectionAsync(string name, bool required)
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
