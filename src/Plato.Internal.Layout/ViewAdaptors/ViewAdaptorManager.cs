using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.ViewAdaptors
{

    public interface IViewAdaptorManager
    {
        Task<IEnumerable<IViewAdaptorResult>> GetViewAdaptors(string name);
    }

    public class ViewAdaptorManager : IViewAdaptorManager
    {

        private readonly ConcurrentDictionary<string, IViewAdaptorResult> _viewAdaptorResults
            = new ConcurrentDictionary<string, IViewAdaptorResult>();

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
                    matchingAdapatorResults.Add(viewAdaptorResult.Value);
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
                                _viewAdaptorResults.TryAdd(viewAdaptorResult.Builder.ViewName, viewAdaptorResult);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"An exception occurred whilst attempting to adapt the view: {provider.ViewName}");
                        }
                    }
                }

            }
            
        }

    }
}
