using System.Linq;
using System.Text;
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

        [HtmlAttributeName("allow-razor-expressions")]
        public bool AllowRazorExpressions { get; set; } = true;

        [HtmlAttributeName("normalize-whitespace")]
        public bool NormalizeWhitespace { get; set; } = true;

        [HtmlAttributeName("markdown")]
        public ModelExpression Markdown { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private readonly IBroker _broker;

        public MarkdownBodyTagHelper(
            IBroker broker)
        {
            _broker = broker;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            await base.ProcessAsync(context, output);

            string content = null;
            if (Markdown != null)
            {
                content = Markdown.Model?.ToString();
            }

            // NullHtmlEncoder allows for Razor Expressions
            // to be parsed before the markdown is parsed 
            // AllowRazorExpressions should only ever be true
            // if we are not handling user supplied input
            var htmlEncoder = AllowRazorExpressions
                ? NullHtmlEncoder.Default
                : HtmlEncoder.Default;

            if (content == null)
            {
                content = (await output.GetChildContentAsync(htmlEncoder))
                    .GetContent(htmlEncoder);
            }
            

            if (string.IsNullOrEmpty(content))
            {
                return;
            }
                
            var markdown = NormalizeWhiteSpaceText(content.Trim('\n', '\r'));

            output.TagName = "div";
            output.Attributes.Add("class", "markdown-body");

            output.Content.SetHtmlContent(await ParseEntityHtml(markdown));
            
        }
        
        async Task<string> ParseEntityHtml(string message)
        {
      
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                message = await handler.Invoke(new Message<string>(message, this));
            }

            return message.HtmlTextulize();

        }

        string NormalizeWhiteSpaceText(string text)
        {

            if (!NormalizeWhitespace || string.IsNullOrEmpty(text))
            {
                return text;
            }
                
            var lines = GetLines(text);
            if (lines.Length < 1)
            {
                return text;
            }
                
            string line1 = null;

            // find first non-empty line
            foreach (var line in lines)
            {
                line1 = line;
                if (!string.IsNullOrEmpty(line1))
                    break;
            }

            if (string.IsNullOrEmpty(line1))
            {
                return text;
            }
                
            var trimLine = line1.TrimStart();
            var whitespaceCount = line1.Length - trimLine.Length;
            if (whitespaceCount == 0)
            {
                return text;
            }
                
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line.Length > whitespaceCount
                    ? line.Substring(whitespaceCount)
                    : line);
            }

            return sb.ToString();
        }

        string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null)
            {
                return null;
            }
                
            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
            {
                return s.Split(new char[] { '\n' });
            }
          
            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();

        }

    }

}
