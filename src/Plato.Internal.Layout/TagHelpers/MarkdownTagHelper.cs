using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Messaging.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

  
    public class MarkdownTagHelper : TagHelper
    {

        [HtmlAttributeName("markdown")]
        public string Markdown { get; set; }


        private readonly IBroker _broker;

        public MarkdownTagHelper(
            IBroker broker)
        {
            _broker = broker;
        }

        public override async Task ProcessAsync(
            TagHelperContext context,
            TagHelperOutput output)
        {

            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

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
