using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Utility.Extensions
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

    }
}
