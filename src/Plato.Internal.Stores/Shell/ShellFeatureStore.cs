using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

            return await _featureRepository.InsertUpdateAsync(feature);

        }

        public async Task<ShellFeature> UpdateAsync(ShellFeature feature)
        {
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
            return await _featureRepository.SelectByIdAsync(id);
        }

        public IQuery<ShellFeature> QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<ShellFeature>> SelectAsync(params object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ShellFeature>> SelectFeatures()
        {
            return await _featureRepository.SelectFeatures();
        }
    }
}
