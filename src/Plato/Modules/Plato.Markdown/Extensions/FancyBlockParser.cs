using System;
using System.Linq;
using System.Text;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.EntityFrameworkCore.Internal;

namespace Plato.Markdown.Extensions
{
    public class FancyBlockParser : BlockParser
    {

        private string[] AllowedCssClasses = new string[]
        {
            "primary",
            "secondary",
            "info",
            "success",
            "warning",
            "danger"
        };

        public FancyBlockParser()
        {
            OpeningCharacters = new[]
            {
                '>'
            };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var column = processor.Column;
            var sourcePosition = processor.Start;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus
            // (a) the character > together with a following space, or
            // (b) a single character > not followed by a space.
            // (c) a single character > followed by a css class > !primary quote
            var currentChar = processor.CurrentChar;
            var c = processor.NextChar();
            if (c.IsSpaceOrTab())
            {
                c = processor.NextChar();
            }

            var cssClass = "";
            if (c == '!')
            {
                while (true)
                {
                    if (c.IsWhitespace())
                    {
                        break;
                    }
                    c = processor.NextChar();
                    cssClass += c;
                }
            }
          
            if (c.IsSpaceOrTab())
            {
                processor.NextColumn();
            }

            // Build quote
            var quoteBlock = new QuoteBlock(this)
            {
                QuoteChar = currentChar,
                Column = column,
                Span = new SourceSpan(sourcePosition, processor.Line.End),
            };

            if (!string.IsNullOrEmpty(cssClass))
            {
                quoteBlock.GetAttributes().AddClass(cssClass);
            }

            // Allows support for...
            // > {primary} quote here
            // > {secondary} quote here
            // > {info} quote here
            // etc...
            //foreach (var cssClass in AllowedCssClasses)
            //{
            //    var pattern = new StringBuilder();
            //    pattern.Append("> {").Append(cssClass).Append("}");

            //    for (var i = 0; i < quoteBlock.Count; i++)
            //    {
            //        var subBlock = quoteBlock[i];

            //    }

            //    if (processor.Line.Text.IndexOf(pattern.ToString(), StringComparison.OrdinalIgnoreCase) >= 0)
            //    {
                  
            //    }
            //}

            processor.NewBlocks.Push(quoteBlock);

            return BlockState.Continue;

        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            var quote = (QuoteBlock)block;

            // 5.1 Block quotes 
            // A block quote marker consists of 0-3 spaces of initial indent, plus
            // (a) the character > together with a following space, or
            // (b) a single character > not followed by a space.
            var c = processor.CurrentChar;
            if (c != quote.QuoteChar)
            {
                return processor.IsBlankLine
                    ? BlockState.BreakDiscard
                    : BlockState.None;
            }

            c = processor.NextChar(); // Skip opening char
            if (c.IsSpace())
            {
                processor.NextChar(); // Skip following space
            }


            block.UpdateSpanEnd(processor.Line.End);
            return BlockState.Continue;
        }
    }


}
