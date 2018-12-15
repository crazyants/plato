using System;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Hosting.Web
{
    
    public class CapturedRouter : ICapturedRouter
    {
        
        private readonly CapturedRouterOptions _options;

        public CapturedRouterOptions Options => _options;
        
        public CapturedRouter()
        {
            _options = new CapturedRouterOptions();
        }
        
        public ICapturedRouter Configure(Action<CapturedRouterOptions> configure)
        {
            configure(_options);
            return this;
        }
    
    }
    
}
