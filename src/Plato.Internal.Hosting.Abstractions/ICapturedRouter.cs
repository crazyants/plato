using System;
using Microsoft.AspNetCore.Routing;

namespace Plato.Internal.Hosting.Abstractions
{
    
    public interface ICapturedRouter
    {

        CapturedRouterOptions Options { get; }

        ICapturedRouter Configure(Action<CapturedRouterOptions> configure);
        
    }
    
    public class CapturedRouterOptions
    {

        public IRouter Router { get; set; }

        public string BaseUrl { get; set; }

    }
    
}
