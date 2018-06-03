using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Plato.Abstractions.Extensions
{

    public static class EnumerableExtensions
    {

        public static string Serialize<T>(this IEnumerable<T> iterator) 
        {
           return JsonConvert.SerializeObject(iterator);
        }

    }


}
