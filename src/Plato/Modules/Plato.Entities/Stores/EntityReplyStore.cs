using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Stores
{
    public class EntityReplyStore<TModel> : IEntityReplyStore<TModel> where TModel : class, IEntityReply
    {

        private readonly ICacheManager _cacheManager;
        private readonly IEntityReplyRepository<TModel> _entityReplyRepository;
        private readonly ILogger<EntityReplyStore<TModel>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityReplyStore(
            ILogger<EntityReplyStore<TModel>> logger,
            IDbQueryConfiguration dbQuery,
            IEntityReplyRepository<TModel> entityReplyRepository,
            ICacheManager cacheManager)
        {
            _logger = logger;
            _dbQuery = dbQuery;
            _entityReplyRepository = entityReplyRepository;
            _cacheManager = cacheManager;
        }

        public async Task<TModel> CreateAsync(TModel reply)
        {

            var newReply = await _entityReplyRepository.InsertUpdateAsync(reply);
            if (newReply != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added entity reply with id {0} for entity { 1}",
                        newReply.Id, newReply.EntityId);
                }
                //_cacheManager.CancelTokens(typeof(EntityStore), reply.EntityId);
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), reply.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), reply.Id);
            }

            return newReply;
        }

        public async Task<TModel> UpdateAsync(TModel reply)
        {

            var updatedReply = await _entityReplyRepository.InsertUpdateAsync(reply);
            if (updatedReply != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Update entity reply with id {1}",
                       updatedReply.Id);
                }
                //_cacheManager.CancelTokens(typeof(EntityStore), reply.EntityId);
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), reply.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), reply.Id);
            }

            return updatedReply;
        }

        public async Task<bool> DeleteAsync(TModel reply)
        {
            var success = await _entityReplyRepository.DeleteAsync(reply.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity reply with id {0} for entity { 1}",
                        reply.Id, reply.EntityId);
                }
                //_cacheManager.CancelTokens(typeof(EntityStore), reply.EntityId);
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), reply.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), reply.Id);
            }

            return success;

        }

        public async Task<TModel> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var reply = await _entityReplyRepository.SelectByIdAsync(id);
                if (reply != null)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Selecting entity reply for key '{0}' with id {1}",
                            token.ToString(), id);
                    }
                }

                return reply;
            });

        }

        public IQuery<TModel> QueryAsync()
        {
            var query = new EntityReplyQuery<TModel>(this);
            return _dbQuery.ConfigureQuery<TModel>(query); ;
        }

        public async Task<IPagedResults<TModel>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting entity replies for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                return await _entityReplyRepository.SelectAsync(args);

            });

        }


    }

}
