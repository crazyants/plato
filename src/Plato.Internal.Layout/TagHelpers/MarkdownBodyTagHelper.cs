using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement("markdown-body")]
    public class MarkdownBodyTagHelper : TagHelper
    {

        [HtmlAttributeName("markdown")]
        public string Markdown { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }


        private readonly IBroker _broker;

        public MarkdownBodyTagHelper(
            IBroker broker)
        {
            _broker = broker;
        }

        public override async Task ProcessAsync(
            TagHelperContext context,
            TagHelperOutput output)
        {
         
            //this.Markdown = HtmlEncoder.Default.Encode(this.Markdown);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "markdown-body");

            output.Content.SetHtmlContent(await ParseEntityHtml(this.Markdown));
            
        }


        async Task<string> ParseEntityHtml(string message)
        {

            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }

            return message.HtmlTextulize();

        }

    }

}
