using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plato.Abstractions.Extensions
{
    public static class StringExtensions
    {

        private static readonly char[] _inValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

        public static bool IsValidUrlSegment(this string segment)
        {       
            return !segment.ContainsCharacters(_inValidSegmentChars);
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
            if (input.EndsWith(trim.ToString()))            
                input = input.TrimEnd(trim);
            return input;

        }
        
        public static Stream StringToStream(this string input)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(input);
            return new MemoryStream(byteArray);
        }
        

        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<T> DeserializeAsync<T>(this string json)
        {
            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json)); 
        }


    }

}
