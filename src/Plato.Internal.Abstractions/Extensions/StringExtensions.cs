using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Plato.Internal.Abstractions.Extensions
{

    public static class StringExtensions
    {
     
        public static readonly char[] InValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

        public static readonly string[] CommonWords = new string[]
        {
            "able",
            "all",
            "am",
            "an",
            "and",
            "about",
            "after",
            "also",
            "any",
            "are",
            "but",
            "be",
            "etc",
            "being",
            "your",
            "with",
            "within",
            "will",
            "can",
            "did",
            "does",
            "for",
            "from",
            "go",
            "good",
            "had",
            "has",
            "have",
            "here",
            "how",
            "i",
            "kind",
            "know",
            "more",
            "most",
            "make",
            "many",
            "must",
            "must",
            "not",
            "never",
            "now",
            "or",
            "other",
            "our",
            "out",
            "over",
            "like",
            "re",
            "said",
            "same",
            "seem",
            "see",
            "should",
            "since",
            "so",
            "some",
            "been",
            "still",
            "such",
            "take",
            "than",
            "that",
            "the",
            "their",
            "them",
            "then",
            "there",
            "they",
            "this",
            "through",
            "too",
            "to",
            "use",
            "under",
            "unable",
            "using",
            "used",
            "up",
            "very",
            "what",
            "went",
            "was",
            "way",
            "well",
            "were",
            "which",
            "while",
            "when",
            "where",
            "who",
            "why",
            "one",
            "two",
            "three",
            "four",
            "five",
            "www",
            "com",
            "info",
            "net",
            "index",
            "http",
            "https",
            "html",
            "you",
            "get",
            "just",
            "last",
            "must",
            "me",
            "my",
            "may"
        };
          
        // ---------

        public static bool IsValidUrlSegment(this string segment)
        {       
            return !segment.ContainsCharacters(InValidSegmentChars);
        }

        public static bool ContainsCharacters(this string subject, params char[] chars)
        {

            if (string.IsNullOrEmpty(subject) || chars == null || chars.Length == 0)            
                return false;
            
            Array.Sort(chars);

            foreach (var current in subject)
            {
                if (Array.BinarySearch(chars, current) >= 0)                
                    return true;
            }

            return false;
        }
        
        public static string ToEmptyIfNull(this string input)
        {
            return input ?? string.Empty;
        }
        
        public static string TrimToSize(this string input, int length)
        {

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (input == null)
            {
                return string.Empty;
            }
                
            return input.Length >= length 
                ? input.Substring(0, length) 
                : input;
        }
        
        public static string TrimToAround(this string input, int length)
        {

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if (input == null)
            {
                return string.Empty;
            }
                
            // we don't have spaces to split
            if (input.IndexOf(' ') == -1)
            {
                return input.Length >= length
                    ? input.Substring(0, length - 3) + "..."
                    : input;
            }

            var oLength = 0;
            var words = input.Split(' ');

            var output = new List<string>(words.Length);
            foreach (var word in words)
            {
                var len = word.Length;
                if ((len + oLength) <= length)
                {
                    output.Add(word);
                    oLength += len + 1;
                }
                else
                {
                    break;
                }
            }

            var sb = new StringBuilder();
            for (var i = 0; i <= output.Count - 1; i++)
            {
                sb.Append((string) output[i]);
                if (i < output.Count - 1)
                {
                    sb.Append(" ");
                }
            }

            output.Clear();

            if (input.Length >= length)
            {
                if (String.IsNullOrWhiteSpace(sb.ToString()))
                {
                    return string.Empty;
                }
                return sb.ToString() + "...";
            }

            return sb.ToString();

        }
        
        public static int[] ToIntArray(this string input, char delimiter = ',')
        {

            if (string.IsNullOrEmpty(input))
                return null;

            int[] output = null;
            var array = input.Split(delimiter);
            output = new int[array.Length];
            for (var i = 0; i <= array.Length - 1; i++)
            {
                var ok = int.TryParse(array.GetValue(i).ToString(), out var id);
                if (ok)
                    output.SetValue(id, i);
            }

            return output;

        }
        
        public static string TryTrimEnd(this string input, char trim)
        {
            return input.EndsWith(trim.ToString()) ? input.TrimEnd(trim) : input;
        }
        
        public static Stream StringToStream(this string input)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(input));
        }
        
        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<T> DeserializeAsync<T>(this string json)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json));
        }

        public static string PlainTextulize(this string input)
        {
            return input.StripNewLines().StripHtml();
        }

        public static string NormalizeNewLines(this string input)
        {
            // Normalize non-unix "\r\n" to standardized "\n"
            return Regex.Replace(input, "\r\n", "\n");
        }

        public static string HtmlTextulize(this string input)
        {
            return Regex.Replace(input.NormalizeNewLines(), "\n", "<br/>");
        }
        
        public static string StripNewLines(this string input)
        {
            return input.Replace(System.Environment.NewLine, ""); ;
        }

        public static string StripHtml(this string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input.IndexOf('<') >= 0 && input.IndexOf('>') >= 0)
            {
                input = Regex.Replace(input, "<[^>]*.>", " ");
            }

            if (input.IndexOf('&') >= 0)
            {
                input = Regex.Replace(input, "&nbsp;", " ", RegexOptions.IgnoreCase);
                input = Regex.Replace(input, "&amp;", "&", RegexOptions.IgnoreCase);
            }

            return input.Trim();

        }

        public static string ToSafeFileName(this string input)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(input, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static string Repeat(this string input, int times)
        {

            if (times <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(times);
            for (var i = 1; i <= times; i++)
            {
                sb.Append(input);
            }
            return sb.ToString();
        }

        public static bool IsBase64String(this string value)
        {

            if (string.IsNullOrEmpty(value) || value.Length % 4 != 0
                                            || value.Contains(' ') || value.Contains('\t') || value.Contains('\r') ||
                                            value.Contains('\n'))
            {
                return false;
            }
                
            var index = value.Length - 1;
            if (value[index] == '=')
            {
                index--;
            }
               
            if (value[index] == '=')
            {
                index--;
            }
               
            for (var i = 0; i <= index; i++)
            {
                if (((char)value[i]).IsValidBase64Char())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        public static string HighlightTerms(this string input, string words)
        {
      
            // ensure we have keywords to highlight
            if (String.IsNullOrEmpty(words))
            {
                return input;
            }

            const string startTag = "<span class=\"bg-highlight\">";
            const string endTag = "</span>";

            // strip junk from words            
            words = Regex.Replace(words, "([^\\w+\\s\\'\\-.])", "");

            // split terms to highlight
            var terms = words.Trim().Split(' ');
            for (var i = 0; i <= terms.Length - 1; i++)
            {

                var term = terms.GetValue(i).ToString();
                if (!String.IsNullOrEmpty(term))
                {
                    term = term.Trim();
                }

                // find the word to highlight ensuring we search outside of HTML tags 
                // if the word is 2 or fewer characters ensure we use word boundaries
                var searchPattern = term.Length <= 2
                    ? "(?!<.*?)\\b(" + Regex.Escape(term) + ")\\b(?![^<>]*?>)"
                    : "(?!<.*?)(" + Regex.Escape(term) + ")(?![^<>]*?>)";
                
                input = Regex.Replace(
                    input,
                    searchPattern,
                    startTag + "$1" + endTag,
                    RegexOptions.IgnoreCase);

            }

            return input;

        }

        public static bool IsValidHex(this string chars)
        {

            if (string.IsNullOrEmpty(chars))
            {
                return false;
            }

            foreach (var c in chars)
            {
                var isHex = ((c >= '0' && c <= '9') ||
                             (c >= 'a' && c <= 'f') ||
                             (c >= 'A' && c <= 'F'));
                if (!isHex)
                {
                    return false;
                }
            }
            return true;
        }
        
        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }

        public static IEnumerable<string> ToDistinctList(this string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }

            // Split input at spaces
            var words = input.Split(' ');

            // Build output
            var output = new List<string>();
            foreach (var word in words)
            {
                // Ensure unique
                if (!output.Contains(word))
                {
                    output.Add(word);
                }
            }

            return output;

        }

        public static string StripCommonWords(this string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            
            if (input.Length <= 2)
            {
                return input;
            }
                
            var inputLower = input.ToLower();

            var i = 0;
            while (i < CommonWords.Length - 1)
            {

                var commonTerm = System.Convert.ToString(CommonWords[i]);

                // is term present within our input?
                if (inputLower.IndexOf(commonTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {

                    // match terms at start of input
                    input = Regex.Replace(input,
                        "^([" + commonTerm + "\\-]+)\\s", " ", RegexOptions.IgnoreCase);

                    // match terms in middle of input
                    input = Regex.Replace(input,
                        "[\\b\\n\\r\\s^]" + commonTerm + "[\\b\\n\\r\\s]", " ", RegexOptions.IgnoreCase);

                    // match terms at end of input
                    input = Regex.Replace(input,
                        "\\s([" + commonTerm + "\\-]+)$", " ", RegexOptions.IgnoreCase);

                }

                i += 1;

            }

            return input;
            
        }

        public static Version ToVersion(this string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var ok = Version.TryParse(input.Trim(), out var version);
            return ok ? version : null;

        }

        public static int[] ToIntArray(this string[] input)
        {
            var output = new List<int>();
            foreach (var value in input)
            {
                if (int.TryParse(value, out var id))
                {
                    output.Add(id);
                }
            }
            return output.ToArray();
        }

        public static int TotalWords(this string input)
        {

            var count = 0;
            var wasInWord = false;
            var inWord = false;

            foreach (var c in input)
            {
                if (inWord)
                {
                    wasInWord = true;
                }

                if (char.IsWhiteSpace(c))
                {
                    if (wasInWord)
                    {
                        count++;
                        wasInWord = false;
                    }
                    inWord = false;
                }
                else
                {
                    inWord = true;
                }
            }
            
            if (wasInWord)
            {
                count++;
            }

            return count;

        }

    }

}
