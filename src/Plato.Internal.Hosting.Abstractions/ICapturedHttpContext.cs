using System;
using Microsoft.AspNetCore.Http;

namespace Plato.Internal.Hosting.Abstractions
{

    public interface ICapturedHttpContext
    {
        CapturedHttpContextState State { get; }

        ICapturedHttpContext Configure(Action<CapturedHttpContextState> configure);

    }

    public class CapturedHttpContextState
    {

        private static HttpContext _httpContext;

        public HttpContext HttpContext => _httpContext;

        public void Contextualize(HttpContext httpContext)
        {
            if (_httpContext == null)
            {
                _httpContext = httpContext;
            }

        }
    }

}
