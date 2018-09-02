using System;
using Microsoft.AspNetCore.Http;

namespace Plato.Internal.Abstractions.Extensions
{
    public static class HttpContextAccessorExtensions
    {

        public static T GetRequestHeaderValueAs<T>(
            this IHttpContextAccessor httpContextAccessor, 
            string headerName)
        {
            if (httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                var rawValues = values.ToString();  
                if (!String.IsNullOrWhiteSpace(rawValues))
                {
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
                }
                    
            }
            return default(T);
        }

    }
}
