using Markdig;

namespace Plato.Markdown.Extensions
{
    public static class MarkdownPipelineBuilderExtensions
    {
        
        public static MarkdownPipelineBuilder UseFancyQuotes(
            this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<FancyQuoteExtension>();
            return pipeline;
        }
    }
}
