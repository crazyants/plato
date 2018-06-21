using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("card")]
    public class CardTagHelper : TagHelper
    {

        public LocalizedHtmlString Title { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var content = await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.Add("class", "panel");

            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(Build(content));

        }

        public IHtmlContent Build(IHtmlContent innerContent)
        {

            var builder = new HtmlContentBuilder();

            var htmlContentBuilder = builder.AppendHtml("<div class=\"card\">");

            if (Title != null)
            {
                htmlContentBuilder
                    .AppendHtml("<div class=\"card-header\">")
                    .AppendHtml(Title.Value)
                    .AppendHtml("</div>");
            }
      
            htmlContentBuilder
                .AppendHtml("<div class=\"card-body\">")
                .AppendHtml(innerContent)
                .AppendHtml("</div>")
                .AppendHtml("</div>");
            

            return htmlContentBuilder.ToHtmlString();


        }


    }
}
