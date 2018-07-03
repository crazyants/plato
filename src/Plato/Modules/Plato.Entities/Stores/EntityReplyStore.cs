using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Cache;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{
    
    public interface IEntityReplyStore<TModel> : IStore<TModel> where TModel : class
    {

    }

    public class EntityReplyStore : IEntityReplyStore<EntityReply>
    {

        private string _key = "EntityReply";

        private readonly IBroker _broker;
        private readonly IEntityReplyRepository<EntityReply> _entityReplyRepository;
        private readonly ILogger<EntityReplyStore> _logger;
        private readonly ICacheDependency _cacheDependency;
        private readonly IMemoryCache _memoryCache;
        private readonly IDbQuery _dbQuery;
            
        public EntityReplyStore(
            ILogger<EntityReplyStore> logger,
            ICacheDependency cacheDependency, 
            IMemoryCache memoryCache, 
            IDbQuery dbQuery,
            IEntityReplyRepository<EntityReply> entityReplyRepository, IBroker broker)
        {
            _logger = logger;
            _cacheDependency = cacheDependency;
            _memoryCache = memoryCache;
            _dbQuery = dbQuery;
            _entityReplyRepository = entityReplyRepository;
            _broker = broker;
        }


        public async Task<EntityReply> CreateAsync(EntityReply reply)
        {

            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
            var newReply = await _entityReplyRepository.InsertUpdateAsync(reply);
            if (newReply != null)
            {
                _cacheDependency.CancelToken(GetEntityReplyCacheKey());
            }

            return newReply;
        }

        public async Task<EntityReply> UpdateAsync(EntityReply reply)
        {

            // Parse Html and message abstract
            reply.Html = await ParseMarkdown(reply.Message);
            reply.Abstract = await ParseAbstract(reply.Message);
            
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

        public IQuery QueryAsync()
        {
            var query = new EntityReplyQuery(this);
            return _dbQuery.ConfigureQuery(query); ;
        }

        public async Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            var key = GetEntityReplyCacheKey();
            return await _memoryCache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                var output = await _entityReplyRepository.SelectAsync<T>(args);
                if (output != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogDebug("Adding entity replies to cache of type {0}. Entry key: {1}.",
                            _memoryCache.GetType().Name, key);
                    }
                }
                cacheEntry.ExpirationTokens.Add(_cacheDependency.GetToken(key));
                return output;
            });

        }
        
        private async Task<string> ParseMarkdown(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseMarkdown"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message;

        }

        async Task<string> ParseAbstract(string message)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseAbstract"
            }, message))
            {
                return await handler.Invoke(new Message<string>(message, this));
            }

            return message.StripHtml().TrimToAround(500);

        }


        string GetEntityReplyCacheKey()
        {
            return $"{_key}";
        }



    }
}
