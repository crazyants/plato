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

    public class TagStore<TModel> : ITagStore<TModel> where TModel : class, ITag
    {
        
        public const string Name = "ByName";
        public const string NameNormalized = "ByNameNormalized";

        private readonly ITagRepository<TModel> _tagRepository;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<TModel> _logger;
       
        public TagStore(
            ITagRepository<TModel> tagRepository,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager,
            ILogger<TModel> logger)
        {
            _tagRepository = tagRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
        }
  
        public async Task<TModel> CreateAsync(TModel model)
        {
            var result = await _tagRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<TModel> UpdateAsync(TModel model)
        {

            var result = await _tagRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(model);
            }

            return result;
        }

        public async Task<bool> DeleteAsync(TModel model)
        {
            var success = await _tagRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted email '{0}' with id {1}",
                        model.Name, model.Id);
                }
                CancelTokens(model);
            }

            return success;
        }
        
        public async Task<TModel> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new InvalidEnumArgumentException(nameof(id));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByIdAsync(id));
        }
        
        public IQuery<TModel> QueryAsync()
        {
            var query = new TagQuery<TModel>(this);
            return _dbQuery.ConfigureQuery<TModel>(query); ;
        }

        public async Task<IPagedResults<TModel>> SelectAsync(DbParam[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting emails for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                return await _tagRepository.SelectAsync(dbParams);

            });
        }

        public async Task<IEnumerable<TModel>> GetByFeatureIdAsync(int featureId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByFeatureIdAsync(featureId));
        }

        public async Task<TModel> GetByNameAsync(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new InvalidEnumArgumentException(nameof(name));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), Name, name);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByNameAsync(name));

        }

        public async Task<TModel> GetByNameNormalizedAsync(string nameNormalized)
        {
            if (String.IsNullOrEmpty(nameNormalized))
            {
                throw new InvalidEnumArgumentException(nameof(nameNormalized));
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), NameNormalized, nameNormalized);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) => await _tagRepository.SelectByNameNormalizedAsync(nameNormalized));

        }

        void CancelTokens(TModel model)
        {
            // Current type
            _cacheManager.CancelTokens(this.GetType());

            // Base type
            if (this.GetType() != typeof(TagStore<TagBase>))
            {
                _cacheManager.CancelTokens(typeof(TagStore<TagBase>));
            }
            
        }

    }

}
