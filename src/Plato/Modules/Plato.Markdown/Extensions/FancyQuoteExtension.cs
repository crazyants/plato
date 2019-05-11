using Markdig;
using Markdig.Renderers;

namespace Plato.Markdown.Extensions
{
    
    public class FancyQuoteExtension : IMarkdownExtension
    {

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FancyQuoteParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new FancyQuoteParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<FancyQuoteRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new FancyQuoteRenderer());
                }
            }
        }

    }
    
}
