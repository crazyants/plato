using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class EnumerableExtensions
    {

        public static string Serialize<T>(this IEnumerable<T> input) 
        {
           return JsonConvert.SerializeObject(input);
        }

        public static IEnumerable<string> StripCommonWords(this IEnumerable<string> input)
        {

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var inputList = input.ToList();
            foreach (var term in StringExtensions.CommonWords)
            {
                if (inputList.Contains(term))
                {
                    inputList.Remove(term);
                }
            }

            return inputList;

        }
    }
    
}
