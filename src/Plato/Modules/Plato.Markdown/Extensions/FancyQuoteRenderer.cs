using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Plato.Markdown.Extensions
{

    public class FancyQuoteRenderer : HtmlObjectRenderer<QuoteBlock>
    {

        protected override void Write(HtmlRenderer renderer, QuoteBlock obj)
        {

            var attrs = obj.GetAttributes();

            renderer.EnsureLine();
            if (renderer.EnableHtmlForBlock)
            {
                renderer.Write("<blockquote").WriteAttributes(obj).WriteLine(">");
            }
            var savedImplicitParagraph = renderer.ImplicitParagraph;
            renderer.ImplicitParagraph = false;
            renderer.WriteChildren(obj);
            renderer.ImplicitParagraph = savedImplicitParagraph;
            if (renderer.EnableHtmlForBlock)
            {
                renderer.WriteLine("</blockquote>");
            }
            renderer.EnsureLine();
        }

    }
    
}
