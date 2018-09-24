using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.ViewAdaptors
{
    
    public class ViewAdaptorManager : IViewAdaptorManager
    {

        private readonly ConcurrentDictionary<string, IList<IViewAdaptorResult>> _viewAdaptorResults
            = new ConcurrentDictionary<string, IList<IViewAdaptorResult>>();

        private readonly IList<IViewAdaptorProvider> _viewAdaptorProviders;
        private readonly ILogger<ViewAdaptorManager> _logger;
   
        public ViewAdaptorManager(
            IEnumerable<IViewAdaptorProvider> viewAdaptorProviders, 
            ILogger<ViewAdaptorManager> logger)
        {
            _viewAdaptorProviders = viewAdaptorProviders.ToList();
            _logger = logger;
        }

        public async Task<IEnumerable<IViewAdaptorResult>> GetViewAdaptors(string viewName)
        {

            // Populate all providers
            await EnsureConfiguredAProviders();

            // Find providers matching our view name
            // Hot code path - avoid linq
            var matchingAdapatorResults = new List<IViewAdaptorResult>();
            foreach (var viewAdaptorResult in _viewAdaptorResults)
            {
                if (viewAdaptorResult.Key.Equals(viewName))
                {
                    matchingAdapatorResults.AddRange(viewAdaptorResult.Value);
                }
            }

            return matchingAdapatorResults;
            
        }
        
        async Task EnsureConfiguredAProviders()
        {

            if (_viewAdaptorResults.Count == 0)
            {
                if (_viewAdaptorProviders?.Count > 0)
                {
                    foreach (var provider in _viewAdaptorProviders)
                    {
                        try
                        {
                            var viewAdaptorResult = await provider.ConfigureAsync();
                            if (viewAdaptorResult != null)
                            {
                                _viewAdaptorResults.AddOrUpdate(viewAdaptorResult.Builder.ViewName,
                                    new List<IViewAdaptorResult>()
                                    {
                                        viewAdaptorResult
                                    }, (k, v) =>
                                    {
                                        v.Add(viewAdaptorResult);
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
