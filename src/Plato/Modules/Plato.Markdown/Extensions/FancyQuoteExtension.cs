using Markdig;
using Markdig.Renderers;

namespace Plato.Markdown.Extensions
{

    // ----------------
    // A custom mark dig extension to add support for styled block quotes. 
    // ----------------

    public class FancyQuoteExtension : IMarkdownExtension
    {

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<FancyBlockParser>())
            {
                // Insert the parser before any other parsers
                pipeline.BlockParsers.Insert(0, new FancyBlockParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<FancyBlockRenderer>())
                {
                    // Must be inserted before CodeBlockRenderer
                    htmlRenderer.ObjectRenderers.Insert(0, new FancyBlockRenderer());
                }
            }
        }
    }
    
}
