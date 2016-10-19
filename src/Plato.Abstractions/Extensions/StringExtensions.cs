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

        private static readonly char[] inValidSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();

        public static bool IsValidUrlSegment(this string segment)
        {       
            return !segment.ContainsCharacters(inValidSegmentChars);
        }

        public static bool ContainsCharacters(this string subject, params char[] chars)
        {

            if (string.IsNullOrEmpty(subject) || chars == null || chars.Length == 0)            
                return false;
            
            Array.Sort(chars);

            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.BinarySearch(chars, current) >= 0)                
                    return true;                
            }

            return false;
        }


        public static string ToEmptyIfNull(this string input)
        {
            return input == null ? string.Empty : input;
            
        }
        
        public static int[] ToIntArray(this string input, char delimiter = ',')
        {

            if (string.IsNullOrEmpty(input))
                return null;

            int[] output = null;
            if (input != null)
            {
                string[] array = input.Split(delimiter);
                output = new int[array.Length];
                for (int i = 0; i <= array.Length - 1; i++)
                {
                    int id = 0;
                    bool ok = Int32.TryParse(array.GetValue(i).ToString(), out id);
                    if (ok)
                        output.SetValue(id, i);
                }
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
                

    }

}
