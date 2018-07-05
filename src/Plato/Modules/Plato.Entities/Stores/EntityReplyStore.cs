using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityReplyStore<TModel> : IStore<TModel> where TModel : class
    {

    }

    public class EntityReplyStore : IEntityReplyStore<EntityReply>
    {

        private string _key = "EntityReply";
        
        private readonly IEntityReplyRepository<EntityReply> _entityReplyRepository;
        private readonly ILogger<EntityReplyStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQueryConfiguration _dbQuery;
            
        public EntityReplyStore(
            ILogger<EntityReplyStore> logger,
            ICacheDependency cacheDependency, 
            IMemoryCache memoryCache,
            IDbQueryConfiguration dbQuery,
            IEntityReplyRepository<EntityReply> entityReplyRepository)
        {
            _logger = logger;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _dbQuery = dbQuery;
            _entityReplyRepository = entityReplyRepository;
        }
        
        public async Task<EntityReply> CreateAsync(EntityReply reply)
        {

            var newReply = await _entityReplyRepository.InsertUpdateAsync(reply);
            if (newReply != null)
            {
                _cacheDependency.CancelToken(GetEntityReplyCacheKey());
            }

            return newReply;
        }

        public async Task<EntityReply> UpdateAsync(EntityReply reply)
        {

            var updatedReply = await _entityReplyRepository.InsertUpdateAsync(reply);
            if (updatedReply != null)
            {
                _cacheDependency.CancelToken(GetEntityReplyCacheKey());
            }

            return updatedReply;
        }

        public async Task<bool> DeleteAsync(EntityReply reply)
        {
            var success = await _entityReplyRepository.DeleteAsync(reply.Id);
            if (success)
            {
                _cacheDependency.CancelToken(GetEntityReplyCacheKey());
            }


            return success;

        }

        public async Task<EntityReply> GetByIdAsync(int id)
        {
            var reply = await _entityReplyRepository.SelectByIdAsync(id);
            if (reply != null)
            {

            }
            return reply;

        }

        public IQuery<EntityReply> QueryAsync()
        {
            var query = new EntityReplyQuery(this);
            return _dbQuery.ConfigureQuery< EntityReply>(query); ;
        }
        
        public async Task<IPagedResults<EntityReply>> SelectAsync(params object[] args)
        {
            var hash = args.GetHashCode().ToString();
            var key = GetEntityReplyCacheKey(hash);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Selecting entity replies for key '{0}' with the following parameters: {1}",
                    key, args.Select(a => a));
            }

            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var output = await _entityReplyRepository.SelectAsync(args);
                if (output != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Adding entity replies to cache with key: {0}", key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return output;
            });

        }

        string GetCacheHashCode(params object[] args)
        {

            if ((args == null) || (args.Length == 0))
            {
                return string.Empty;
            }

            // Precalculate buffer size & ensure GetHashCode is only ever called once
            var codes = new List<int>();
            var len = 0;
            foreach (var arg in args)
            {
                // An argument can be null
                if (arg != null)
                {
                    var code = arg.GetHashCode();
                    len += code.ToString().Length;
                    codes.Add(code);
                }
            }

            var sb = new StringBuilder(len);
            foreach (var code in codes)
            {
                  sb.Append(code);
            }

            return sb.ToString();

        }

        string GetEntityReplyCacheKey()
        {
            return $"{_key}";
        }

        string GetEntityReplyCacheKey(string hash)
        {
            return $"{_key}_{hash}";
        }
        
    }
}
