using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Titles
{
    
    public class PageTitleBuilder : IPageTitleBuilder
    {

        private readonly List<PageTitlePart> _parts;
        private IHtmlContent _title;

        public PageTitleBuilder()
        {
            _parts = new List<PageTitlePart>();
        }

        public void Clear()
        {
            _parts.Clear();
        }

        public void AddSegment(IHtmlContent segment, int position = 0)
        {
            _title = null;
            _parts.Add(new PageTitlePart
            {
                Value = segment,
                Position = position
            });
        }

        public void AddSegments(IEnumerable<IHtmlContent> parts, int position)
        {
            foreach (var part in parts)
            {
                AddSegment(part, position);
            }
        }
        
        public IHtmlContent GenerateTitle(IHtmlContent separator)
        {
            if (_title != null)
            {
                return _title;
            }

            if (separator == null)
            {
                separator = new HtmlString(" - ");
            }
            
            var builder = new HtmlContentBuilder();

            if (_parts.Count == 0)
            {
                return HtmlString.Empty;
            }

            _parts.Sort();
            for (var i = 0; i < _parts.Count; i++)
            {
                builder.AppendHtml(_parts[i].Value);
                if (i < _parts.Count - 1)
                {
                    builder.AppendHtml(separator);
                }
            }

            _title = builder;

            return _title;

        }
    }

    public class PageTitlePart : IComparable
    {

        public int Position { get; set; }

        public IHtmlContent Value { get; set; }

        public int CompareTo(object other)
        {
            if (other == null)
                return 1;
            var sortOrderCompare = ((PageTitlePart)other).Position;
            if (this.Position == sortOrderCompare)
                return 0;
            if (this.Position < sortOrderCompare)
                return -1;
            if (this.Position > sortOrderCompare)
                return 1;
            return 0;
        }
    }
}
