using System;
using System.Collections.Generic;

namespace Plato.Internal.Text.Abstractions
{
    public interface IHtmlSanitizer
    {
        string Sanitize(string html);

        string Sanitize(string html, Action<SanitizerOptions> configure);
    }

    public class SanitizerOptions
    {
        public string[] BlackList { get; set; } = new string[]
        {
            "script",
            "iframe",
            "frame",
            "object",
            "embed",
            "form"
        };
        
    }

}
