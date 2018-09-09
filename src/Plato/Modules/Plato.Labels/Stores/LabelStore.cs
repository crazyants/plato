using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Labels.Models;
using Plato.Labels.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Labels.Stores
{

    public class LabelStore<TLabel> : ILabelStore<TLabel> where TLabel : class, ILabel
    {

        private readonly ILabelRepository<TLabel> _LabelRepository;
        private readonly ILabelDataStore<LabelData> _LabelDataStore;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<LabelStore<TLabel>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public LabelStore(
            ILabelRepository<TLabel> LabelRepository,
            ICacheManager cacheManager,
            ILogger<LabelStore<TLabel>> logger,
            IDbQueryConfiguration dbQuery,
            ILabelDataStore<LabelData> LabelDataStore, ITypedModuleProvider typedModuleProvider)
        {
            _LabelRepository = LabelRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
            _LabelDataStore = LabelDataStore;
            _typedModuleProvider = typedModuleProvider;
        }

        #region "Implementation"

        public async Task<TLabel> CreateAsync(TLabel model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var newLabel = await _LabelRepository.InsertUpdateAsync(model);
            if (newLabel != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added new Label with id {1}",
                        newLabel.Id);
                }

                _cacheManager.CancelTokens(this.GetType());
            }

            return newLabel;

        }

        public async Task<TLabel> UpdateAsync(TLabel model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var updatedLabel = await _LabelRepository.InsertUpdateAsync(model);
            if (updatedLabel != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated existing entity with id {1}",
                        updatedLabel.Id);
                }

                _cacheManager.CancelTokens(this.GetType());

            }

            return updatedLabel;

        }

        public async Task<bool> DeleteAsync(TLabel model)
        {

            var success = await _LabelRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted Label '{0}' with id {1}",
                        model.Name, model.Id);
                }
                _cacheManager.CancelTokens(this.GetType());
            }

            return success;

        }

        public async Task<TLabel> GetByIdAsync(int id)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var Label = await _LabelRepository.SelectByIdAsync(id);
                return await MergeLabelData(Label);
            });
        }

        public IQuery<TLabel> QueryAsync()
        {
            var query = new LabelQuery<TLabel>(this);
            return _dbQuery.ConfigureQuery<TLabel>(query); ;
        }

        public async Task<IPagedResults<TLabel>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting categories for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                var results = await _LabelRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await MergeLabelData(results.Data);
                }

                return results;

            });
        }

        public async Task<IEnumerable<TLabel>> GetByFeatureIdAsync(int featureId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting labels for feature with Id '{0}'",
                        featureId);
                }

                var results = await _LabelRepository.SelectByFeatureIdAsync(featureId);
                if (results != null)
                {
                    results = await MergeLabelData(results.ToList());
                }

                return results;

            });
        }

        #endregion

        #region "Private Methods"

        async Task<IEnumerable<LabelData>> SerializeMetaDataAsync(TLabel Label)
        {

            // Get all existing entity data
            var data = await _LabelDataStore.GetByLabelIdAsync(Label.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<LabelData>();

            // Iterate all meta data on the supplied object,
            // check if a key already exists, if so update existing key 
            var output = new List<LabelData>();
            foreach (var item in Label.MetaData)
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

        async Task<IList<TLabel>> MergeLabelData(IList<TLabel> categories)
        {

            if (categories == null)
            {
                return null;
            }

            // Get all entity data matching supplied entity ids
            var results = await _LabelDataStore.QueryAsync()
                .Select<LabelDataQueryParams>(q => { q.LabelId.IsIn(categories.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return categories;
            }

            // Merge data into entities
            return await MergeLabelData(categories, results.Data);

        }

        async Task<IList<TLabel>> MergeLabelData(IList<TLabel> categories, IList<LabelData> data)
        {

            if (categories == null || data == null)
            {
                return categories;
            }

            for (var i = 0; i < categories.Count; i++)
            {
                categories[i].Data = data.Where(d => d.LabelId == categories[i].Id).ToList();
                categories[i] = await MergeLabelData(categories[i]);
            }

            return categories;

        }

        async Task<TLabel> MergeLabelData(TLabel Label)
        {

            if (Label == null)
            {
                return null;
            }

            if (Label.Data == null)
            {
                return Label;
            }

            foreach (var data in Label.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    Label.AddOrUpdate(type, (ISerializable)obj);
                }
            }

            return Label;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

        #endregion

    }

    //public class LabelStore : ILabelStore<Label>
    //{

    //    private readonly ILabelRepository<Label> _LabelRepository;
    //    private readonly ILabelDataStore<LabelData> _LabelDataStore;
    //    private readonly ICacheManager _cacheManager;
    //    private readonly ILogger<LabelStore> _logger;
    //    private readonly IDbQueryConfiguration _dbQuery;
    //    private readonly ITypedModuleProvider _typedModuleProvider;

    //    public LabelStore(
    //        ILabelRepository<Label> LabelRepository,
    //        ICacheManager cacheManager,
    //        ILogger<LabelStore> logger,
    //        IDbQueryConfiguration dbQuery,
    //        ILabelDataStore<LabelData> LabelDataStore, ITypedModuleProvider typedModuleProvider)
    //    {
    //        _LabelRepository = LabelRepository;
    //        _cacheManager = cacheManager;
    //        _logger = logger;
    //        _dbQuery = dbQuery;
    //        _LabelDataStore = LabelDataStore;
    //        _typedModuleProvider = typedModuleProvider;
    //    }

    //    #region "Implementation"

    //    public async Task<Label> CreateAsync(Label model)
    //    {
            
    //        // transform meta data
    //        model.Data = await SerializeMetaDataAsync(model);
            
    //        var newLabel = await _LabelRepository.InsertUpdateAsync(model);
    //        if (newLabel != null)
    //        {
    //            if (_logger.IsEnabled(LogLevel.Information))
    //            {
    //                _logger.LogInformation("Added new Label with id {1}",
    //                    newLabel.Id);
    //            }

    //            _cacheManager.CancelTokens(this.GetType());
    //        }

    //        return newLabel;

    //    }

    //    public async Task<Label> UpdateAsync(Label model)
    //    {
            
    //        // transform meta data
    //        model.Data = await SerializeMetaDataAsync(model);
            
    //        var updatedLabel = await _LabelRepository.InsertUpdateAsync(model);
    //        if (updatedLabel != null)
    //        {
    //            if (_logger.IsEnabled(LogLevel.Information))
    //            {
    //                _logger.LogInformation("Updated existing entity with id {1}",
    //                    updatedLabel.Id);
    //            }

    //            _cacheManager.CancelTokens(this.GetType());

    //        }

    //        return updatedLabel;

    //    }

    //    public async Task<bool> DeleteAsync(Label model)
    //    {

    //        var success = await _LabelRepository.DeleteAsync(model.Id);
    //        if (success)
    //        {
    //            if (_logger.IsEnabled(LogLevel.Information))
    //            {
    //                _logger.LogInformation("Deleted Label '{0}' with id {1}",
    //                    model.Name, model.Id);
    //            }
    //            _cacheManager.CancelTokens(this.GetType());
    //        }

    //        return success;

    //    }

    //    public async Task<Label> GetByIdAsync(int id)
    //    {
    //        var token = _cacheManager.GetOrCreateToken(this.GetType(), id);
    //        return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
    //        {
    //            var Label = await _LabelRepository.SelectByIdAsync(id);
    //            return await MergeLabelData(Label);
    //        });
    //    }

    //    public IQuery<Label> QueryAsync()
    //    {
    //        var query = new LabelQuery(this);
    //        return _dbQuery.ConfigureQuery<Label>(query); ;
    //    }

    //    public async Task<IPagedResults<Label>> SelectAsync(params object[] args)
    //    {
    //        var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
    //        return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
    //        {

    //            if (_logger.IsEnabled(LogLevel.Information))
    //            {
    //                _logger.LogInformation("Selecting categories for key '{0}' with the following parameters: {1}",
    //                    token.ToString(), args.Select(a => a));
    //            }

    //            var results =  await _LabelRepository.SelectAsync(args);
    //            if (results != null)
    //            {
    //                results.Data = await MergeLabelData(results.Data);
    //            }

    //            return results;

    //        });
    //    }
        
    //    public async Task<IEnumerable<Label>> GetByFeatureIdAsync(int featureId)
    //    {
    //        var token = _cacheManager.GetOrCreateToken(this.GetType(), featureId);
    //        return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
    //        {

    //            if (_logger.IsEnabled(LogLevel.Information))
    //            {
    //                _logger.LogInformation("Selecting categories for feature with Id '{0}'",
    //                    featureId);
    //            }

    //            var results = await _LabelRepository.SelectByFeatureIdAsync(featureId);
    //            if (results != null)
    //            {
    //                results = await MergeLabelData(results.ToList());
    //            }

    //            return results;

    //        });
    //    }

    //    #endregion

    //    #region "Private Methods"

    //    async Task<IEnumerable<LabelData>> SerializeMetaDataAsync(Label Label)
    //    {

    //        // Get all existing entity data
    //        var data = await _LabelDataStore.GetByLabelIdAsync(Label.Id);

    //        // Prepare list to search, use dummy list if needed
    //        var dataList = data?.ToList() ?? new List<LabelData>();

    //        // Iterate all meta data on the supplied object,
    //        // check if a key already exists, if so update existing key 
    //        var output = new List<LabelData>();
    //        foreach (var item in Label.MetaData)
    //        {
    //            var key = item.Key.FullName;
    //            var entityData = dataList.FirstOrDefault(d => d.Key == key);
    //            if (entityData != null)
    //            {
    //                entityData.Value = item.Value.Serialize();
    //            }
    //            else
    //            {
    //                entityData = new LabelData()
    //                {
    //                    Key = key,
    //                    Value = item.Value.Serialize()
    //                };
    //            }

    //            output.Add(entityData);
    //        }

    //        return output;

    //    }

    //    async Task<IList<Label>> MergeLabelData(IList<Label> categories)
    //    {

    //        if (categories == null)
    //        {
    //            return null;
    //        }

    //        // Get all entity data matching supplied entity ids
    //        var results = await _LabelDataStore.QueryAsync()
    //            .Select<LabelDataQueryParams>(q => { q.LabelId.IsIn(categories.Select(e => e.Id).ToArray()); })
    //            .ToList();

    //        if (results == null)
    //        {
    //            return categories;
    //        }

    //        // Merge data into entities
    //        return await MergeLabelData(categories, results.Data);

    //    }

    //    async Task<IList<Label>> MergeLabelData(IList<Label> categories, IList<LabelData> data)
    //    {

    //        if (categories == null || data == null)
    //        {
    //            return categories;
    //        }

    //        for (var i = 0; i < categories.Count; i++)
    //        {
    //            categories[i].Data = data.Where(d => d.LabelId == categories[i].Id).ToList();
    //            categories[i] = await MergeLabelData(categories[i]);
    //        }

    //        return categories;

    //    }

    //    async Task<Label> MergeLabelData(Label Label)
    //    {

    //        if (Label == null)
    //        {
    //            return null;
    //        }

    //        if (Label.Data == null)
    //        {
    //            return Label;
    //        }

    //        foreach (var data in Label.Data)
    //        {
    //            var type = await GetModuleTypeCandidateAsync(data.Key);
    //            if (type != null)
    //            {
    //                var obj = JsonConvert.DeserializeObject(data.Value, type);
    //                Label.AddOrUpdate(type, (ISerializable)obj);
    //            }
    //        }

    //        return Label;

    //    }

    //    async Task<Type> GetModuleTypeCandidateAsync(string typeName)
    //    {
    //        return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
    //    }

    //    #endregion

    //}
}
