using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;

namespace Plato.Internal.Abstractions.Extensions
{

    public static class CollectionExtensions
    {

        public static IReadOnlyList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            // ToArray trims the excess space and speeds up access
            return new ReadOnlyCollection<T>(new List<T>(enumerable).ToArray());
        }
        
        public static IDictionary<string, string> JsonToDictionary(this string json)
        { 
            return JsonConvert.DeserializeObject<IDictionary<string, string>>(json);
        }

        public static string DictionaryToJson(this IDictionary<string, string> dictionary)
        {

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (MemoryStream ms = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {               
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartObject();
                        foreach (var entry in dictionary)
                        {
                            writer.WritePropertyName(entry.Key);
                            writer.WriteValue(entry.Value);
                        }
                        writer.WriteEndObject();                   
                    }

                }

                return sb.ToString();


            }


        }
        
    }
}
