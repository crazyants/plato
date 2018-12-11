using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Internal.Text
{
    
    public class HtmlSanitizer : IHtmlSanitizer
    {

        public static readonly IDictionary<string, string> HtmlEntities = new Dictionary<string, string>()
        {
            ["A"] = "&#65;",
            ["B"] = "&#66;",
            ["C"] = "&#67;",
            ["D"] = "&#68;",
            ["E"] = "&#69;",
            ["F"] = "&#70;",
            ["G"] = "&#71;",
            ["H"] = "&#72;",
            ["I"] = "&#73;",
            ["J"] = "&#74;",
            ["K"] = "&#75;",
            ["L"] = "&#76;",
            ["M"] = "&#77;",
            ["N"] = "&#78;",
            ["O"] = "&#79;",
            ["P"] = "&#80;",
            ["Q"] = "&#81;",
            ["R"] = "&#82;",
            ["S"] = "&#83;",
            ["T"] = "&#84;",
            ["U"] = "&#85;",
            ["V"] = "&#86;",
            ["W"] = "&#87;",
            ["X"] = "&#88;",
            ["Y"] = "&#89;",
            ["Z"] = "&#90;",
            ["a"] = "&#97;",
            ["b"] = "&#98;",
            ["c"] = "&#99;",
            ["d"] = "&#100;",
            ["e"] = "&#101;",
            ["f"] = "&#102;",
            ["g"] = "&#103;",
            ["h"] = "&#104;",
            ["i"] = "&#105;",
            ["j"] = "&#106;",
            ["k"] = "&#107;",
            ["l"] = "&#108;",
            ["m"] = "&#109;",
            ["n"] = "&#110;",
            ["o"] = "&#111;",
            ["p"] = "&#112;",
            ["q"] = "&#113;",
            ["r"] = "&#114;",
            ["s"] = "&#115;",
            ["t"] = "&#116;",
            ["u"] = "&#117;",
            ["v"] = "&#118;",
            ["w"] = "&#119;",
            ["x"] = "&#120;",
            ["y"] = "&#121;",
            ["z"] = "&#122;"
        };
        
        private readonly SanitizerOptions _options;

        //private static readonly RegexOptions _opts = RegexOptions.IgnoreCase | RegexOptions.Multiline;

        //static string HtmlSanitizeTagBlackList { get; } = "script|iframe|frame|object|embed|form";

        //private static readonly Regex RegExScript = 
        //    new Regex($@"(<({HtmlSanitizeTagBlackList})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({HtmlSanitizeTagBlackList})>)", _opts);

        //// strip unsupported: and unicode representation of unsupported:
        //// href='unsupported:alert(\"gotcha\")'
        //// href='&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;:alert(\"gotcha\");'
        //private static readonly Regex RegExJavaScriptHref = 
        //    new Regex(@"<.*?(href|src|dynsrc|lowsrc)=.{0,20}((ᴶavascript:)|(&#)).*?>", _opts);

        //private static readonly Regex RegExOnEventAttributes = 
        //    new Regex(@"on(.{4,12})=", _opts);

        //private static readonly Regex RegExMarkdownCodeBlocks =
        //    new Regex("(`{1,3}[\n|\r]*)([^`{1,3}].*?)([\n|\r]*`{1,3})", _opts);


        public HtmlSanitizer()
        {
            _options = new SanitizerOptions();
        }

        #region "Implementation"

        public string Sanitize(string html)
        {
            return SanitizeInternal(html);
        }

        public string Sanitize(string html, Action<SanitizerOptions> configure)
        {
            configure(_options);
            return SanitizeInternal(html);
        }

        #endregion

        #region "Private Methods"

        string SanitizeInternal(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            //// First attempt some basic encoding 
            ////html = EncodeBlackListedTags(html);
            //html = EncodeOnEventHandlers(html);
            //html = EncodeGlobalObjects(html);

            //// Next remove anything left over
            //html = RemoveJavaScriptdirective(html);
            ////html = RemoveBlackListed(html);
            /// 
            return html;

        }

        //string EncodeBlackListedTags(string input)
        //{
        
        //    // Perform basic html encoding for black listed tags
        //    foreach (var tag in _options.BlackList)
        //    {
        //        var encodedTag = ToHtmlEncoded(tag);
        //        input = Regex.Replace(input, $"<{tag}([^>].*)>", $"<{encodedTag}>", _opts);
        //        input = Regex.Replace(input, $"<\\/{tag}[^><].*>", $"</{encodedTag}>", _opts);
        //        input = Regex.Replace(input, $"<{tag}>", $"<{encodedTag}>", _opts);
        //        input = Regex.Replace(input, $"<\\/{tag}>", $"<{encodedTag}>", _opts);
        //        input = Regex.Replace(input, $"<{tag}", $"<{encodedTag}", _opts);
        //        input = Regex.Replace(input, $"<\\/{tag}", $"</{encodedTag}", _opts);
        //        input = Regex.Replace(input, $"{tag}>", $"{encodedTag}>", _opts);
        //        input = Regex.Replace(input, $"\\/{tag}>", $"/{encodedTag}>", _opts);
        //    }


        //    var codeBlockMatches = RegExMarkdownCodeBlocks.Matches(input);

        //    foreach (Match match in codeBlockMatches)
        //    {
        //        var matchingText = match.Value;
        //        if (match.Groups.Count > 1)
        //        {
        //            var start = match.Groups[1].Value;
        //            var middle = match.Groups[2].Value;
        //            var end = match.Groups[3].Value;
        //            middle = ToHtmlEncoded(middle);

        //            var searchPattern = match.Value;
        //            input = Regex.Replace(
        //                input,
        //                matchingText,
        //                start + middle + end,
        //                _opts);
        //        }
        //    }

          
        //    return input;

        //}
        
        //public string ToHtmlEncoded(string tag)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var c in tag)
        //    {
        //        if (HtmlEntities.ContainsKey(c.ToString()))
        //        {
        //            sb.Append(HtmlEntities[c.ToString()]);
        //        }
        //        else
        //        {
        //            sb.Append(c.ToString());
        //        }
                
        //    }

        //    return sb.ToString();
        //}
        
        //string EncodeOnEventHandlers(string input)
        //{
        //    // Remove onEvent handlers from elements
        //    foreach (Match match in RegExOnEventAttributes.Matches(input))
        //    {
        //        var txt = match.Value;
        //        if (match.Groups.Count > 1)
        //        {
        //            var searchPattern = match.Value;
        //            input = Regex.Replace(
        //                input,
        //                searchPattern,
        //                $"&#111;n{match.Groups[1].Value}=",
        //                _opts);
        //        }
        //    }

        //    return input;

        //}

        //string RemoveJavaScriptdirective(string input)
        //{
 
        //    foreach (Match match in RegExJavaScriptHref.Matches(input))
        //    {
        //        if (match.Groups.Count > 2)
        //        {
        //            var txt = match.Value.Replace(match.Groups[2].Value, "'#'");
        //            input = input.Replace(match.Value, txt);
        //        }
        //    }

        //    return input;

        //}

        //string EncodeGlobalObjects(string input)
        //{
        //    input = Regex.Replace(input, "window\\.", "&#119;indow.", _opts);
        //    input = Regex.Replace(input, "document\\.", "&#100;ocument.", _opts);
        //    return input;
        //}

        //string RemoveBlackListed(string input)
        //{

        //    // Nect remove blacklisted tags if any unencoded versions still exist
        //    var htmlTagSearchPattern = String.Join("|", _options.BlackList);
        //    if (!string.IsNullOrEmpty(htmlTagSearchPattern) || htmlTagSearchPattern == HtmlSanitizeTagBlackList)
        //    {
        //        // Replace tags - reused expr is more efficient
        //        input = RegExScript.Replace(input, string.Empty);
        //    }
        //    else
        //    {
        //        input = Regex.Replace(input,
        //            $@"(<({htmlTagSearchPattern})\b[^<]*(?:(?!<\/({HtmlSanitizeTagBlackList}))<[^<]*)*<\/({htmlTagSearchPattern})>)",
        //            "", _opts);
        //    }

        //    return input;

        //}

        #endregion

    }

}
