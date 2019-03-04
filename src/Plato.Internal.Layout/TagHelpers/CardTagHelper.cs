using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plato.Internal.Layout.TagHelpers
{
    
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
            
            output.TagName = "div";
            if (this.Enabled)
            {
                output.Attributes.Add("class", $"card card-{Type.ToString().ToLower()}");
            }

            // title
            if (cardContext.Title != null && this.Enabled)
            {
                var panelTitle = new TagBuilder("div");
                panelTitle.AddCssClass(cardContext.Title.CssClass);
                panelTitle.InnerHtml.AppendHtml(cardContext.Title.Content);
                output.Content.AppendHtml(panelTitle);
            }
            
            if (cardContext.Body != null)
            {
                // panel body
                if (cardContext.Body.Content != null && this.Enabled)
                {
                    var panelBody = new TagBuilder("div");
                    panelBody.AddCssClass(cardContext.Body.CssClass);
                    panelBody.InnerHtml.AppendHtml(cardContext.Body.Content);
                    output.Content.AppendHtml(panelBody);
                }
                else
                {
                    output.Content.SetHtmlContent(cardContext.Body.Content);
                    output.Attributes.Clear();
                }
            }
           

            // panel footer
            if (cardContext.Footer != null && this.Enabled)
            {
                var panelFooter = new TagBuilder("div");
                panelFooter.AddCssClass(cardContext.Footer.CssClass);
                panelFooter.InnerHtml.AppendHtml(cardContext.Footer.Content);

                output.Content.AppendHtml(panelFooter);
            }

        }
        
    }
    
    public enum CardType
    {
        Default,
        Primary,
        Success,
        Info,
        Warning,
        Danger
    }
    
}
