using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Internal.Resources
{
    
    public class ResourceManager : IResourceManager
    {

        private readonly IEnumerable<IResourceProvider> _resourceProviders;
        private readonly ILogger<ResourceManager> _logger;

        public ResourceManager(
            IEnumerable<IResourceProvider> resourceProviders,
            ILogger<ResourceManager> logger)
        {
            _resourceProviders = resourceProviders;
            _logger = logger;
        }


        public async Task<IEnumerable<ResourceGroup>> GetResources()
        {

            var output = new List<ResourceGroup>();
            foreach (var provider in _resourceProviders)
            {
                try
                {
                    var resources = await provider.GetResourceGroups();
                    if (resources != null)
                    {
                        output.AddRange(resources);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred whilst attempting to execute a resource provider of type {provider.GetType()}.");
                }
            }

            return output;
        }

    }
}
