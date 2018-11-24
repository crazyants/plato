using System;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Hosting.Web
{
    
    public class CapturedRouter : ICapturedRouter
    {
        
        private CapturedRouterOptions _options;

        public CapturedRouterOptions Options => _options;
        
        public CapturedRouter()
        {
        }
        
        public ICapturedRouter Configure(Action<CapturedRouterOptions> configure)
        {
            var options = new CapturedRouterOptions();
            configure(options);
            _options = options;
            return this;
        }
    
    }
    
}
