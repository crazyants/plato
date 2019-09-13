using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.ViewAdapters
{
    
    public class ViewAdapterManager : IViewAdapterManager
    {

        private readonly IEnumerable<IViewAdapterProvider> _viewAdapterProviders;
        private readonly ILogger<ViewAdapterManager> _logger;
   
        public ViewAdapterManager(
            IEnumerable<IViewAdapterProvider> viewAdapterProviders, 
            ILogger<ViewAdapterManager> logger)
        {
            _viewAdapterProviders = viewAdapterProviders;
            _logger = logger;
        }

        public async Task<IEnumerable<IViewAdapterResult>> GetViewAdaptersAsync(string viewName)
        {
            
            List<IViewAdapterResult> matchingAdapterResults = null;
            foreach (var provider in _viewAdapterProviders)
            {
                try
                {
                    var viewAdapterResult = await provider.ConfigureAsync(viewName);
                    if (viewAdapterResult != null)
                    {
                        if (matchingAdapterResults == null)
                        {
                            matchingAdapterResults = new List<IViewAdapterResult>();
                        }                        
                        matchingAdapterResults.Add(viewAdapterResult);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"An exception occurred whilst attempting to adapt the view: {provider.ViewName}");
                }
            }
                                 
            return matchingAdapterResults;
            
        }
        
    }

}
