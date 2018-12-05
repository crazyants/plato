using System;
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


        public const string ByName = "ByName";
        public const string ByNameNormalized = "ByNameNormalized";

        private readonly ITagRepository<Tag> _tagRepository;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<TagStore> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public TagStore(
            ITagRepository<Tag> tagRepository,
            ICacheManager cacheManager,
            ILogger<TagStore> logger,
            IDbQueryConfiguration dbQuery)
        {
            _tagRepository = tagRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }
        
        #region "Implementation
        
        public async Task<Tag> CreateAsync(Tag model)
        {
            var result = await _tagRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<Tag> UpdateAsync(Tag model)
        {

            var result = await _tagRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                _cacheManager.CancelTokens(this.GetType());
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Tag model)
        {
            var success = await _tagRepository.DeleteAsync(model.Id);
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
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByIdAsync(id));
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

                return await _tagRepository.SelectAsync(args);

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

                return await _tagRepository.SelectByFeatureIdAsync(featureId);
                
            });
        }

        public async Task<Tag> GetByNameAsync(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new InvalidEnumArgumentException(nameof(name));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByName, name);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByNameAsync(name));

        }

        public async Task<Tag> GetByNameNormalizedAsync(string nameNormalized)
        {
            if (String.IsNullOrEmpty(nameNormalized))
            {
                throw new InvalidEnumArgumentException(nameof(nameNormalized));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByNameNormalized, nameNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByNameNormalizedAsync(nameNormalized));

        }

        #endregion

    }

}
