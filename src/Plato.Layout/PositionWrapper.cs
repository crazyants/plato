using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Plato.Layout
{
    public class PositionWrapper : IHtmlContent, IPosition
    {
        private IHtmlContent _value;
        public string Position { get; set; }

        public PositionWrapper(IHtmlContent value, string position)
        {
            _value = value;
            Position = position;
        }

        public PositionWrapper(string value, string position)
        {
            _value = new HtmlString(HtmlEncoder.Default.Encode(value));
            Position = position;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            _value.WriteTo(writer, encoder);
        }
    }

}
