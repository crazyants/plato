using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Repositories.Shell;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Internal.Stores.Shell
{

    public class ShellFeatureStore : IShellFeatureStore<ShellFeature>
    {
        private readonly IShellFeatureRepository<ShellFeature> _featureRepository;
        private readonly ILogger<ShellFeatureStore> _logger;
        
        public ShellFeatureStore(
            ILogger<ShellFeatureStore> logger, 
            IShellFeatureRepository<ShellFeature> featureRepository)
        {
            _logger = logger;
            _featureRepository = featureRepository;
        }

        public async Task<ShellFeature> CreateAsync(ShellFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            // Serialize feature settings
            if (feature.FeatureSettings != null)
            {
                feature.Settings = await feature.FeatureSettings.SerializeAsync();
            }

            return await _featureRepository.InsertUpdateAsync(feature);

        }

        public async Task<ShellFeature> UpdateAsync(ShellFeature feature)
        {

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            // Serialize feature settings
            if (feature.FeatureSettings != null)
            {
                feature.Settings = await feature.FeatureSettings.SerializeAsync();
            }


            return await _featureRepository.InsertUpdateAsync(feature);
        }

        public async Task<bool> DeleteAsync(ShellFeature feature)
        {
            var success = await _featureRepository.DeleteAsync(feature.Id);
            if (success)
            {
                //_cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(model.UserId));
            }

            return success;
        }

        public async Task<ShellFeature> GetByIdAsync(int id)
        {

            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            return await _featureRepository.SelectByIdAsync(id);
        }

        public IQuery<ShellFeature> QueryAsync()
        {
            throw new NotImplementedException();
        }
        
        public Task<IPagedResults<ShellFeature>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ShellFeature>> SelectFeatures()
        {

            IList<ShellFeature> output = null;
            var features = await _featureRepository.SelectFeatures();
            if (features != null)
            {
                output = new List<ShellFeature>();
                foreach (var feature in features)
                {
                    feature.FeatureSettings = await feature.Settings.DeserializeAsync<ShellFeatureSettings>();
                    output.Add(feature);
                }
            }

            return output;


        }
    }
}
