using Newtonsoft.Json;
using Plato.Internal.Abstractions;

namespace Plato.Internal.Models.Extensions
{
    public static class ModelExtensions 
    {

        public static string Serialize<T>(this IDbModel<object> model)
        {
            return JsonConvert.SerializeObject(model);
        }

    }
}
