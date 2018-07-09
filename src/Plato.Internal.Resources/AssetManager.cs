using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Assets
{
    
    public class AssetManager : IAssetManager
    {

        private readonly IEnumerable<IAssetProvider> _resourceProviders;
        private readonly ILogger<AssetManager> _logger;

        public AssetManager(
            IEnumerable<IAssetProvider> resourceProviders,
            ILogger<AssetManager> logger)
        {
            _resourceProviders = resourceProviders;
            _logger = logger;
        }


        public async Task<IEnumerable<AssetEnvironment>> GetResources()
        {

            var output = new List<AssetEnvironment>();
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
