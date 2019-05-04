﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.ViewAdapters
{
    
    public class ViewAdapterManager : IViewAdapterManager
    {

        private readonly ConcurrentDictionary<string, IList<IViewAdapterResult>> _viewAdapterResults
            = new ConcurrentDictionary<string, IList<IViewAdapterResult>>();

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

            // Populate all providers
            await EnsureConfiguredProviders();

            // Find providers matching our view name
            var matchingAdapterResults = new List<IViewAdapterResult>();
            foreach (var viewAdapterResult in _viewAdapterResults)
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

            if (_viewAdapterResults.Count == 0)
            {
                if (_viewAdapterProviders != null)
                {
                    foreach (var provider in _viewAdapterProviders)
                    {
                        try
                        {
                            var viewAdapterResult = await provider.ConfigureAsync();
                            if (viewAdapterResult != null)
                            {
                                _viewAdapterResults.AddOrUpdate(viewAdapterResult.Builder.ViewName,
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
