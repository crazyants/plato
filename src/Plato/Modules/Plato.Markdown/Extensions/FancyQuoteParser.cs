using System.Linq;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Plato.Markdown.Extensions
{
    public class FancyQuoteParser : BlockParser
    {

        private readonly string[] _validCssClasses;


        public FancyQuoteParser() : this(null)
        {
        }

        public FancyQuoteParser(string[] validCssClasses)
        {
            _validCssClasses = validCssClasses ?? new []
            {
                "primary",
                "secondary",
                "info",
                "success",
                "warning",
                "danger"
            };
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
            // (c) a single character > followed by a space and explanation mark
            // denoting the css class to apply to the block quote
            // i.e....
            // > !danger This is a dangerous quote 
            // > !success This is a success quote

            var currentChar = processor.CurrentChar;
            var c = processor.NextChar();
            if (c.IsSpaceOrTab())
            {
                c = processor.NextChar();
            }

            var cssClass = "";
            if (c == '!')
            {
                // Parse until we reach white space
                while (true)
                {
                    if (c.IsWhitespace())
                    {
                        break;
                    }
                    c = processor.NextChar();
                    if (!c.IsWhitespace())
                    {
                        cssClass += c;
                    }
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

            // If we found a css class ensure it's allowed
            if (!string.IsNullOrEmpty(cssClass))
            {
                var validCss = cssClass.ToLower();
                if (_validCssClasses.Contains(validCss))
                {
                    quoteBlock.GetAttributes().AddClass(validCss);
                }
            }

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
