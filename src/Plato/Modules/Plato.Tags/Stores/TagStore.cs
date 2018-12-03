using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Repositories;

namespace Plato.Tags.Stores
{

    public class TagStore : ITagStore<Tag>
    {

        private readonly ITagRepository<Tag> _tagsRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<TagStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public TagStore(
            ITagRepository<Tag> tagsRepository,
            ICacheManager cacheManager,
            ILogger<TagStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _tagsRepository = tagsRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }
        
        #region "Implementation
        
        public async Task<Tag> CreateAsync(Tag model)
        {
            var result = await _tagsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<Tag> UpdateAsync(Tag model)
        {

            var result = await _tagsRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Tag model)
        {
            var success = await _tagsRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted email '{0}' with id {1}",
                        model.Name, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;
        }
        
        public async Task<Tag> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagsRepository.SelectByIdAsync(id));
        }
        
        public IQuery<Tag> QueryAsync()
        {
            var query = new TagQuery(this);
            return _dbQuery.ConfigureQuery<Tag>(query); ;
        }

        public async Task<IPagedResults<Tag>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting emails for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _tagsRepository.SelectAsync(args);

            });
        }

        public async Task<IEnumerable<Tag>> GetByFeatureIdAsync(int featureId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting tags for feature with Id '{0}'",
                        featureId);
                }

                return await _tagsRepository.SelectByFeatureIdAsync(featureId);
                
            });
        }
        
        #endregion

    }

}
