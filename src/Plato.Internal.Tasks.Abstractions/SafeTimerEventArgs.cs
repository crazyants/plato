using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Internal.Tasks.Abstractions
{
    public class SafeTimerEventArgs : EventArgs
    {

        public HttpContext HttpContext;

        public SafeTimerEventArgs()
        {
        }

        public SafeTimerEventArgs(HttpContext context)
        {
            HttpContext = context;
        }

    }
    
}
