using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Repositories;

namespace Plato.Labels.Stores
{

    public class LabelStore<TLabel> : ILabelStore<TLabel> where TLabel : class, ILabel
    {

        private const string ById = "ById";
        private const string ByFeatureId = "ByFeatureId";

        private readonly ILabelRepository<TLabel> _labelRepository;
        private readonly ILabelDataStore<LabelData> _labelDataStore;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<LabelStore<TLabel>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public LabelStore(
            ILabelDataStore<LabelData> labelDataStore,
            ILabelRepository<TLabel> labelRepository,
            ITypedModuleProvider typedModuleProvider,
            ILogger<LabelStore<TLabel>> logger,
            IDbQueryConfiguration dbQuery,
            ICacheManager cacheManager)
        {
            _typedModuleProvider = typedModuleProvider;
            _labelRepository = labelRepository;
            _labelDataStore = labelDataStore;
            _cacheManager = cacheManager;
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<TLabel> CreateAsync(TLabel model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var result = await _labelRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<TLabel> UpdateAsync(TLabel model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var result = await _labelRepository.InsertUpdateAsync(model);
            if (result != null)
            {
                CancelTokens(result);
            }

            return result;

        }

        public async Task<bool> DeleteAsync(TLabel model)
        {

            var success = await _labelRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted label '{0}' with id {1}",
                        model.Name, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public async Task<TLabel> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var label = await _labelRepository.SelectByIdAsync(id);
                return await MergeLabelData(label);
            });
        }

        public IQuery<TLabel> QueryAsync()
        {
            var query = new LabelQuery<TLabel>(this);
            return _dbQuery.ConfigureQuery<TLabel>(query); ;
        }

        public async Task<IPagedResults<TLabel>> SelectAsync(IDbDataParameter[] dbParams)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), dbParams.Select(p => p.Value).ToArray());
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting categories for key '{0}' with the following parameters: {1}",
                        token.ToString(), dbParams.Select(p => p.Value));
                }

                var results = await _labelRepository.SelectAsync(dbParams);
                if (results != null)
                {
                    results.Data = await MergeLabelData(results.Data);
                }

                return results;

            });
        }

        public async Task<IEnumerable<TLabel>> GetByFeatureIdAsync(int featureId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByFeatureId, featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting labels for feature with Id '{0}'",
                        featureId);
                }

                var results = await _labelRepository.SelectByFeatureIdAsync(featureId);
                if (results != null)
                {
                    results = await MergeLabelData(results.ToList());
                }

                return results;

            });
        }

        public void CancelTokens(TLabel model)
        {
            CancelTokensInternal(model);
        }

        #endregion

        #region "Private Methods"

        async Task<IEnumerable<LabelData>> SerializeMetaDataAsync(TLabel label)
        {

            // Get all existing label data
            var data = await _labelDataStore.GetByLabelIdAsync(label.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<LabelData>();

            // Iterate all meta data on the supplied object,
            // check if a key already exists, if so update existing key 
            var output = new List<LabelData>();
            foreach (var item in label.MetaData)
            {
                var key = item.Key.FullName;
                var entityData = dataList.FirstOrDefault(d => d.Key == key);
                if (entityData != null)
                {
                    entityData.Value = item.Value.Serialize();
                }
                else
                {
                    entityData = new LabelData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(entityData);
            }

            return output;

        }

        async Task<IList<TLabel>> MergeLabelData(IList<TLabel> labels)
        {

            if (labels == null)
            {
                return null;
            }

            // Get all label data matching supplied label ids
            var results = await _labelDataStore.QueryAsync()
                .Select<LabelDataQueryParams>(q => { q.LabelId.IsIn(labels.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return labels;
            }

            // Merge data into entities
            return await MergeLabelData(labels, results.Data);

        }

        async Task<IList<TLabel>> MergeLabelData(IList<TLabel> labels, IList<LabelData> data)
        {

            if (labels == null || data == null)
            {
                return labels;
            }

            for (var i = 0; i < labels.Count; i++)
            {
                labels[i].Data = data.Where(d => d.LabelId == labels[i].Id).ToList();
                labels[i] = await MergeLabelData(labels[i]);
            }

            return labels;

        }

        async Task<TLabel> MergeLabelData(TLabel label)
        {

            if (label == null)
            {
                return null;
            }

            if (label.Data == null)
            {
                return label;
            }

            foreach (var data in label.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    label.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return label;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

        void CancelTokensInternal(TLabel label)
        {

            // Clear generic type
            _cacheManager.CancelTokens(this.GetType());

            // Clear base type
            if (this.GetType() != typeof(LabelStore<LabelBase>))
            {
                _cacheManager.CancelTokens(typeof(LabelStore<LabelBase>));
            }

            // Clear label data
            _cacheManager.CancelTokens(typeof(LabelDataStore));
            
        }

        #endregion

    }
    
}
