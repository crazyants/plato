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

        private static readonly char[] InValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

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
                var id = 0;
                var ok = int.TryParse(array.GetValue(i).ToString(), out id);
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
        
        public static string StripNewLines(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = input.Replace(System.Environment.NewLine, "");
            input = Regex.Replace(input, @"\r\n?|\n", "");
            return input;
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
                input = Regex.Replace(input, "&nbsp;", "", RegexOptions.IgnoreCase);
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

    }

}
