using System;
using System.Text.RegularExpressions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{
    
    public class HtmlSanitizer : IHtmlSanitizer
    {

        static string HtmlSanitizeTagBlackList { get; } = "script|iframe|frame|object|embed|form";

        private static readonly Regex RegExScript = new Regex($@"(<({HtmlSanitizeTagBlackList})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({HtmlSanitizeTagBlackList})>)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        // strip unsupported: and unicode representation of unsupported:
        // href='unsupported:alert(\"gotcha\")'
        // href='&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;:alert(\"gotcha\");'
        private static readonly Regex RegExJavaScriptHref = new Regex(
            @"<.*?(href|src|dynsrc|lowsrc)=.{0,20}((ᴶavascript:)|(&#)).*?>",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static readonly Regex RegExOnEventAttributes = new Regex(
            @"<.*?\s(on.{4,12}=([""].*?[""]|['].*?['])).*?(>|\/>)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);
        
        public string SanitizeHtml(string html)
        {
            return SanitizeHtml(html, new string[]
            {
                "script",
                "iframe",
                "frame",
                "object",
                "embed",
                "form"
            });
        }

        public string SanitizeHtml(string html, string[] excludeTags)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // First attempt to encode blacklisted tags
            html = TrySimpleEncode(html, excludeTags);

            // Nect remove blacklisted tags if any unencoded versions still exist
            var htmlTagSearchPattern = String.Join("|", excludeTags);
            if (!string.IsNullOrEmpty(htmlTagSearchPattern) || htmlTagSearchPattern == HtmlSanitizeTagBlackList)
            {
                // Replace tags - reused expr is more efficient
                html = RegExScript.Replace(html, string.Empty);
            }
            else
            {
                html = Regex.Replace(html,
                    $@"(<({htmlTagSearchPattern})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({htmlTagSearchPattern})>)",
                    "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }

            // Remove unsupported: directives
            var matches = RegExJavaScriptHref.Matches(html);
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    var txt = match.Value.Replace(match.Groups[2].Value, "'#'");
                    html = html.Replace(match.Value, txt);
                }
            }

            // Remove onEvent handlers from elements
            matches = RegExOnEventAttributes.Matches(html);
            foreach (Match match in matches)
            {
                var txt = match.Value;
                if (match.Groups.Count > 1)
                {
                    var onEvent = match.Groups[1].Value;
                    txt = txt.Replace(onEvent, string.Empty);
                    if (!string.IsNullOrEmpty(txt))
                        html = html.Replace(match.Value, txt);
                }
            }

            return html;

        }

        string TrySimpleEncode(string input, string[] excludeTags)
        {
        
            const RegexOptions opts = RegexOptions.IgnoreCase | RegexOptions.Multiline;

            // Perform basic html encoding for black listed tags
            foreach (var tag in excludeTags)
            {
                input = Regex.Replace(input, $"<{tag}([^>].*)>", $"&lt;{tag}$1&gt;", opts);
                input = Regex.Replace(input, $"<\\/{tag}[^><].*>", $"&lt;/{tag}&gt;", opts);
                input = Regex.Replace(input, $"<{tag}>", $"&lt;{tag}&gt;", opts);
                input = Regex.Replace(input, $"<\\/{tag}>", $"&lt;/{tag}&gt;", opts);
                input = Regex.Replace(input, $"<{tag}", $"&lt;{tag}", opts);
                input = Regex.Replace(input, $"<\\/{tag}", $"&lt;/{tag}", opts);
                input = Regex.Replace(input, $"{tag}>", $"{tag}&gt;", opts);
                input = Regex.Replace(input, $"\\/{tag}>", $"/{tag}&gt;", opts);
            }

            // Encode possible client side events to ensure they are not executed by a browser
            // Replace onabort, onblur, onmove, onload etc to &#111;nload etc
            // Similar to...
            // input = Regex.Replace(input, "onabort=", "&#111;nabort=", opts);
            // input = Regex.Replace(input, "onblur=", "&#111;nblur=", opts);
            // input = Regex.Replace(input, "onchange=", "&#111;nchange=", opts);
            // input = Regex.Replace(input, "onclick=", "&#111;nclick=", opts);
            // input = Regex.Replace(input, "ondblclick=", "&#111;ndblclick=", opts);
            // etc etc etc
            input = Regex.Replace(input, "on([a-z|A-Z]*)=", "&#111;n$1=", opts);
            
            // Encode global objects
            input = Regex.Replace(input, "window\\.", "&#119;indow.", opts);
            input = Regex.Replace(input, "document\\.", "&#100;ocument.", opts);

            return input;

        }

    }

}
