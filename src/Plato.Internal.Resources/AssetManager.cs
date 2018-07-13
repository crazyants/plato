using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Assets
{

    public class AssetManager : IAssetManager
    {
        private readonly IList<AssetEnvironment> _localAssets = new List<AssetEnvironment>();

        private readonly IEnumerable<IAssetProvider> _assetProviders;
        private readonly ILogger<AssetManager> _logger;

        public AssetManager(
            IEnumerable<IAssetProvider> assetProviders,
            ILogger<AssetManager> logger)
        {
            _assetProviders = assetProviders;
            _logger = logger;
        }
        
        public async Task<IEnumerable<AssetEnvironment>> GetAssets()
        {

            var output = new List<AssetEnvironment>();
            foreach (var provider in _assetProviders)
            {
                try
                {
                    var env = await provider.GetAssetGroups();
                    if (env != null)
                    {
                        output.AddRange(env);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred whilst attempting to execute a resource provider of type {provider.GetType()}.");
                }
            }
            
            // do we have any assets set via SetAssets();
            if (_localAssets.Count > 0)
            {
                output.AddRange(_localAssets);
            }
           
            return output;

        }

        public void SetAssets(IEnumerable<AssetEnvironment> environments)
        {
            foreach (var env in environments)
            {
                if (!_localAssets.Contains(env))
                {
                    _localAssets.Add(env);
                }
            }
        }

    }
}
