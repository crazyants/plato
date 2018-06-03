using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace Plato.Abstractions.Extensions
{

    public static class HtmlContentExtensions
    {

        public static HtmlString RenderTag(this IHtmlContent output)
        {
            using (var writer = new StringWriter())
            {
                output.WriteTo(writer, HtmlEncoder.Default);
                return new HtmlString(writer.ToString());
            }
        }

    }
}
