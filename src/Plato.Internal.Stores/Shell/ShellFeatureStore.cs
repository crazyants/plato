using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache.Abstractions;
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
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public ShellFeatureStore(
            IShellFeatureRepository<ShellFeature> featureRepository,
            ILogger<ShellFeatureStore> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _featureRepository = featureRepository;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
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

            var result = await _featureRepository.InsertUpdateAsync(feature);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

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
            
            var result = await _featureRepository.InsertUpdateAsync(feature);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(ShellFeature model)
        {
            var success = await _featureRepository.DeleteAsync(model.Id);
            if (success)
            {
                CancelTokens(model);
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
            var query = new ShellFeatureQuery(this);
            return _dbQuery.ConfigureQuery<ShellFeature>(query); ;
        }
        
        public async Task<IPagedResults<ShellFeature>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token,
                async (cacheEntry) => await _featureRepository.SelectAsync(dbParams));
        }

        public async Task<IEnumerable<ShellFeature>> SelectFeatures()
        {

            var token = _cacheManager.GetOrCreateToken(this.GetType());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                IList<ShellFeature> output = new List<ShellFeature>();
                var features = await _featureRepository.SelectFeatures();
                if (features != null)
                {
                    // Deserialize before storing in cache
                    foreach (var feature in features)
                    {
                        feature.FeatureSettings = await feature.Settings.DeserializeAsync<FeatureSettings>();
                        output.Add(feature);
                    }
                }

                return output;

            });

        }

        public void CancelTokens(ShellFeature model = null)
        {
            _cacheManager.CancelTokens(this.GetType());
        }

    }

}
