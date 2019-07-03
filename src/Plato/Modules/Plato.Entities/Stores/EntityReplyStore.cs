using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Entities.Stores
{
    public class EntityReplyStore<TModel> : IEntityReplyStore<TModel> where TModel : class, IEntityReply
    {

        
        private readonly IEntityReplyDataStore<IEntityReplyData> _entityReplyDataStore;
        private readonly IEntityReplyRepository<TModel> _entityReplyRepository;
        private readonly ILogger<EntityReplyStore<TModel>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;
        
        public EntityReplyStore(
            IEntityReplyDataStore<IEntityReplyData> entityReplyDataStore,
            IEntityReplyRepository<TModel> entityReplyRepository,
            ILogger<EntityReplyStore<TModel>> logger,
            ITypedModuleProvider typedModuleProvider,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _entityReplyRepository = entityReplyRepository;
            _entityReplyDataStore = entityReplyDataStore;
            _typedModuleProvider = typedModuleProvider;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        public async Task<TModel> CreateAsync(TModel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);
            
            var newReply = await _entityReplyRepository.InsertUpdateAsync(model);
            if (newReply != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added entity reply with id {0} for entity { 1}",
                        newReply.Id, newReply.EntityId);
                }
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), model.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), model.Id);
            }

            return await MergeEntityReplyData(newReply); ;
        }

        public async Task<TModel> UpdateAsync(TModel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Transform meta data
            model.Data = await SerializeMetaDataAsync(model);
            
            var updatedReply = await _entityReplyRepository.InsertUpdateAsync(model);
            if (updatedReply != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated entity reply with id {1}",
                       updatedReply.Id);
                }
                //_cacheManager.CancelTokens(typeof(EntityStore), reply.EntityId);
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), model.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), model.Id);
            }

            return await MergeEntityReplyData(updatedReply);
        }

        public async Task<bool> DeleteAsync(TModel model)
        {
            var success = await _entityReplyRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted entity reply with id {0} for entity { 1}",
                        model.Id, model.EntityId);
                }
                //_cacheManager.CancelTokens(typeof(EntityStore), reply.EntityId);
                _cacheManager.CancelTokens(typeof(EntityReplyStore<TModel>), model.EntityId);
                _cacheManager.CancelTokens(this.GetType());
                _cacheManager.CancelTokens(this.GetType(), model.Id);
            }

            return success;

        }

        public async Task<TModel> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var reply = await _entityReplyRepository.SelectByIdAsync(id);
                return await MergeEntityReplyData(reply);
            });
        }

        public IQuery<TModel> QueryAsync()
        {
            var query = new EntityReplyQuery<TModel>(this);
            return _dbQuery.ConfigureQuery<TModel>(query); ;
        }

        public async Task<IPagedResults<TModel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var results = await _entityReplyRepository.SelectAsync(dbParams);
                if (results != null)
                {
                    results.Data = await MergeEntityReplyData(results.Data);
                }
                return results;
            });

        }

        // --------------------------
        
        async Task<IEnumerable<IEntityReplyData>> SerializeMetaDataAsync(TModel entity)
        {

            // Get all existing entity data
            var data = await _entityReplyDataStore.GetByReplyIdAsync(entity.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<IEntityReplyData>();

            // Iterate all meta data on the supplied type,
            // check if a key already exists, if so update existing key 
            var output = new List<IEntityReplyData>();
            foreach (var item in entity.MetaData)
            {
                var key = item.Key.FullName;
                var entityData = dataList.FirstOrDefault(d => d.Key == key);
                if (entityData != null)
                {
                    entityData.Value = await item.Value.SerializeAsync();
                }
                else
                {
                    entityData = new EntityReplyData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(entityData);
            }

            return output;

        }

        async Task<IList<TModel>> MergeEntityReplyData(IList<TModel> replies)
        {

            if (replies == null)
            {
                return null;
            }

            // Get all entity reply data matching supplied entity ids
            var results = await _entityReplyDataStore.QueryAsync()
                .Select<EntityReplyDataQueryParams>(q => { q.ReplyId.IsIn(replies.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return replies;
            }

            // Merge data into entities
            return await MergeEntityData(replies, results.Data);

        }

        async Task<IList<TModel>> MergeEntityData(IList<TModel> replies, IList<IEntityReplyData> data)
        {

            if (replies == null || data == null)
            {
                return replies;
            }

            for (var i = 0; i < replies.Count; i++)
            {
                replies[i].Data = data.Where(d => d.ReplyId == replies[i].Id).ToList();
                replies[i] = await MergeEntityReplyData(replies[i]);
            }

            return replies;

        }

        async Task<TModel> MergeEntityReplyData(TModel reply)
        {

            if (reply == null)
            {
                return null;
            }

            if (reply.Data == null)
            {
                return reply;
            }

            foreach (var data in reply.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    reply.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return reply;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

    }

}
