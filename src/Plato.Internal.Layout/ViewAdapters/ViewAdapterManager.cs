using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.ViewAdapters
{
    
    public class ViewAdapterManager : IViewAdapterManager
    {

        private readonly ConcurrentDictionary<string, IList<IViewAdapterResult>> _viewAdaptorResults
            = new ConcurrentDictionary<string, IList<IViewAdapterResult>>();

        private readonly IList<IViewAdapterProvider> _viewAdapterProviders;
        private readonly ILogger<ViewAdapterManager> _logger;
   
        public ViewAdapterManager(
            IEnumerable<IViewAdapterProvider> viewAdaptrrProviders, 
            ILogger<ViewAdapterManager> logger)
        {
            _viewAdapterProviders = viewAdaptrrProviders.ToList();
            _logger = logger;
        }

        public async Task<IEnumerable<IViewAdapterResult>> GetViewAdapters(string viewName)
        {

            // Populate all providers
            await EnsureConfiguredProviders();

            // Find providers matching our view name
            var matchingAdapterResults = new List<IViewAdapterResult>();
            foreach (var viewAdapterResult in _viewAdaptorResults)
            {
                if (viewAdapterResult.Key.Equals(viewName))
                {
                    matchingAdapterResults.AddRange(viewAdapterResult.Value);
                }
            }

            return matchingAdapterResults;
            
        }
        
        async Task EnsureConfiguredProviders()
        {

            if (_viewAdaptorResults.Count == 0)
            {
                if (_viewAdapterProviders?.Count > 0)
                {
                    foreach (var provider in _viewAdapterProviders)
                    {
                        try
                        {
                            var viewAdapterResult = await provider.ConfigureAsync();
                            if (viewAdapterResult != null)
                            {
                                _viewAdaptorResults.AddOrUpdate(viewAdapterResult.Builder.ViewName,
                                    new List<IViewAdapterResult>()
                                    {
                                        viewAdapterResult
                                    }, (k, v) =>
                                    {
                                        v.Add(viewAdapterResult);
                                        return v;
                                    });
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,
                                $"An exception occurred whilst attempting to adapt the view: {provider.ViewName}");
                        }
                    }
                }

            }
            
        }

    }
}
