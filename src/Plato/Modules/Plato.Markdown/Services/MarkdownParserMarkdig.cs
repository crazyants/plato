using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Plato.Markdown.Extensions;

namespace Plato.Markdown.Services
{

    public class MarkdownParserMarkdig : IMarkdownParser
    {

        public static MarkdownPipeline _pipeline;

        private readonly bool _usePragmaLines;

        public MarkdownParserMarkdig(
            bool usePragmaLines,
            Action<MarkdownPipelineBuilder> config)
        {
            _usePragmaLines = usePragmaLines;
            if (_pipeline == null)
            {
                var builder = CreatePipelineBuilder(config);
                _pipeline = builder.Build();
            }
        }

        #region "Implementation"

        public Task<string> ParseAsync(string markdown)
        {
            if (String.IsNullOrEmpty(markdown))
            {
                return Task.FromResult(string.Empty);
            }
                
            var htmlWriter = new StringWriter();
            var renderer = CreateRenderer(htmlWriter);
            Markdig.Markdown.Convert(markdown, renderer, _pipeline);
            return Task.FromResult(htmlWriter.ToString());

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
                    .UseBootstrap()
                    .UseTaskLists()
                    .UseCustomContainers()
                    .UseGenericAttributes()
                    .UseFancyQuotes()
                    .DisableHtml();

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


        // --------------------------------




    }
}
