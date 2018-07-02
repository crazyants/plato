using System;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;

namespace Plato.Markdown.Services
{

    public class MarkdownParserMarkdig : IMarkdownParser
    {

        public static MarkdownPipeline Pipeline;

        private readonly bool _usePragmaLines;

        public MarkdownParserMarkdig(
            bool usePragmaLines,
            Action<MarkdownPipelineBuilder> config)
        {
            _usePragmaLines = usePragmaLines;
            if (Pipeline == null)
            {
                var builder = CreatePipelineBuilder(config);
                Pipeline = builder.Build();
            }
        }

        #region "Implementation"

        public Task<string> ParseAsync(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return Task.FromResult(string.Empty);

            var htmlWriter = new StringWriter();
            var renderer = CreateRenderer(htmlWriter);

            Markdig.Markdown.Convert(markdown, renderer, Pipeline);

            var html = htmlWriter.ToString();

            //html = ParseFontAwesomeIcons(html);

            //if (!mmApp.Configuration.MarkdownOptions.AllowRenderScriptTags)
            //html = ParseScript(html);

            return Task.FromResult(html);
        }

        #endregion

        #region "Private Methods"

        private MarkdownPipelineBuilder CreatePipelineBuilder(Action<MarkdownPipelineBuilder> config)
        {
            MarkdownPipelineBuilder builder = null;

            // build it explicitly
            if (config == null)
            {
                builder = new MarkdownPipelineBuilder()
                    .UseEmphasisExtras()
                    .UsePipeTables()
                    .UseGridTables()
                    .UseFooters()
                    .UseFootnotes()
                    .UseCitations()
                    .UseAutoLinks() // URLs are parsed into anchors
                    .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
                    .UseAbbreviations()
                    .UseYamlFrontMatter()
                    .UseEmojiAndSmiley(true)
                    .UseMediaLinks()
                    .UseListExtras()
                    .UseFigures()
                    .UseTaskLists()
                    .UseCustomContainers()
                    .UseGenericAttributes();

                //builder = builder.UseSmartyPants();            

                if (_usePragmaLines)
                    builder = builder.UsePragmaLines();

                return builder;
            }


            // let the passed in action configure the builder
            builder = new MarkdownPipelineBuilder();
            config.Invoke(builder);

            if (_usePragmaLines)
                builder = builder.UsePragmaLines();

            return builder;
        }

        private IMarkdownRenderer CreateRenderer(TextWriter writer)
        {
            return new HtmlRenderer(writer);
        }

        #endregion

    }
}
