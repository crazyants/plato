using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{

    public enum CardType
    {
        Default,
        Primary,
        Success,
        Info,
        Warning,
        Danger
    }


    [RestrictChildren("card-title", "card-body", "card-footer")]
    public class CardTagHelper : TagHelper
    {

        [HtmlAttributeName("type")]
        public CardType Type { get; set; }

        [HtmlAttributeName("enabled")]
        public bool Enabled { get; set; } = true;

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var cardContext = new CardContext();
            context.Items.Add(typeof(CardContext), cardContext);

            var content = await output.GetChildContentAsync();

            if (this.Enabled)
            {
                output.TagName = "div";
                output.Attributes.Add("class", $"card card-{Type.ToString().ToLower()}");
            }

            // title
            if (cardContext.Title != null && this.Enabled)
            {
                var panelTitle = new TagBuilder("div");
                panelTitle.AddCssClass("card-header");
                panelTitle.InnerHtml.AppendHtml(cardContext.Title);

                output.Content.AppendHtml(panelTitle);
            }

            // panel body
            if (cardContext.Body != null && this.Enabled)
            {
                var panelBody = new TagBuilder("div");
                panelBody.AddCssClass("card-body");
                panelBody.InnerHtml.AppendHtml(cardContext.Body);

                output.Content.AppendHtml(panelBody);
            }
            else
            {
                output.Content.SetHtmlContent(cardContext.Body);
            }

            // panel footer
            if (cardContext.Footer != null && this.Enabled)
            {
                var panelFooter = new TagBuilder("div");
                panelFooter.AddCssClass("card-footer");
                panelFooter.InnerHtml.AppendHtml(cardContext.Footer);

                output.Content.AppendHtml(panelFooter);
            }

        }
        
    }
}
