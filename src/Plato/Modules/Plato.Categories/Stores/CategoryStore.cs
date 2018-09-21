using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Categories.Models;
using Plato.Categories.Repositories;
using Plato.Internal.Abstractions;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Categories.Stores
{

    public class CategoryStore<TCategory> : ICategoryStore<TCategory> where TCategory : class, ICategory
    {

        public const string ById = "ById";
        public const string ByFeatureId = "ByFeatureId";

        private readonly ICategoryRepository<TCategory> _categoryRepository;
        private readonly ICategoryDataStore<CategoryData> _categoryDataStore;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<CategoryStore<TCategory>> _logger;
        private readonly IDbQueryConfiguration _dbQuery;
        private readonly ICacheManager _cacheManager;

        public CategoryStore(
            ICategoryRepository<TCategory> categoryRepository,
            ICacheManager cacheManager,
            ILogger<CategoryStore<TCategory>> logger,
            IDbQueryConfiguration dbQuery,
            ICategoryDataStore<CategoryData> categoryDataStore, ITypedModuleProvider typedModuleProvider)
        {
            _categoryRepository = categoryRepository;
            _cacheManager = cacheManager;
            _logger = logger;
            _dbQuery = dbQuery;
            _categoryDataStore = categoryDataStore;
            _typedModuleProvider = typedModuleProvider;
        }

        #region "Implementation"

        public virtual async Task<TCategory> CreateAsync(TCategory model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var newCategory = await _categoryRepository.InsertUpdateAsync(model);
            if (newCategory != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Added new category with id {1}",
                        newCategory.Id);
                }

                CancelTokens(newCategory);

            }

            return newCategory;

        }

        public virtual async Task<TCategory> UpdateAsync(TCategory model)
        {

            // transform meta data
            model.Data = await SerializeMetaDataAsync(model);

            var updatedCategory = await _categoryRepository.InsertUpdateAsync(model);
            if (updatedCategory != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Updated existing entity with id {1}",
                        updatedCategory.Id);
                }

                CancelTokens(updatedCategory);

            }

            return updatedCategory;

        }

        public virtual async Task<bool> DeleteAsync(TCategory model)
        {

            var success = await _categoryRepository.DeleteAsync(model.Id);
            if (success)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Deleted category '{0}' with id {1}",
                        model.Name, model.Id);
                }

                CancelTokens(model);

            }

            return success;

        }

        public virtual async Task<TCategory> GetByIdAsync(int id)
        {
            
            if (id <= 0)
            {
                return null;
            }

            var token = _cacheManager.GetOrCreateToken(this.GetType(), ById, id);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {
                var category = await _categoryRepository.SelectByIdAsync(id);
                return await MergeCategoryData(category);
            });

        }

        public virtual IQuery<TCategory> QueryAsync()
        {
            var query = new CategoryQuery<TCategory>(this);
            return _dbQuery.ConfigureQuery<TCategory>(query);
            ;
        }

        public virtual async Task<IPagedResults<TCategory>> SelectAsync(params object[] args)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), args);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting categories for key '{0}' with the following parameters: {1}",
                        token.ToString(), args.Select(a => a));
                }

                var results = await _categoryRepository.SelectAsync(args);
                if (results != null)
                {
                    results.Data = await MergeCategoryData(results.Data);
                }

                return results;

            });
        }

        public virtual async Task<IEnumerable<TCategory>> GetByFeatureIdAsync(int featureId)
        {
            var token = _cacheManager.GetOrCreateToken(this.GetType(), ByFeatureId, featureId);
            return await _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Selecting categories for feature with Id '{0}'",
                        featureId);
                }

                var results = await _categoryRepository.SelectByFeatureIdAsync(featureId);
                if (results != null)
                {
                    results = await MergeCategoryData(results.ToList());
                    results = PrepareChildren(results.ToLookup(c => c.ParentId));
                    results = results.OrderBy(r => r.SortOrder);
                }

                return results;

            });
        }

        public async Task<IEnumerable<TCategory>> GetParentsByIdAsync(int categoryId)
        {

            if (categoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(categoryId));
            }

            var category = await GetByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }

            if (category.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(category.FeatureId));
            }
            
            var categories = await GetByFeatureIdAsync(category.FeatureId);
            if (categories == null)
            {
                return null;
            }

            return RecurseParents(categories.ToList(), category.Id).Reverse();

        }

        public async Task<IEnumerable<TCategory>> GetChildrenByIdAsync(int categoryId)
        {
            var category = await GetByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }

            var categories = await GetByFeatureIdAsync(category.FeatureId);
            if (categories == null)
            {
                return null;
            }

            return RecurseChildren(categories.ToList(), category.Id);

        }
        
        #endregion

        #region "Private Methods"
        
        async Task<IEnumerable<CategoryData>> SerializeMetaDataAsync(TCategory category)
        {

            // Get all existing entity data
            var data = await _categoryDataStore.GetByCategoryIdAsync(category.Id);

            // Prepare list to search, use dummy list if needed
            var dataList = data?.ToList() ?? new List<CategoryData>();

            // Iterate all meta data on the supplied object,
            // check if a key already exists, if so update existing key 
            var output = new List<CategoryData>();
            foreach (var item in category.MetaData)
            {
                var key = item.Key.FullName;
                var entityData = dataList.FirstOrDefault(d => d.Key == key);
                if (entityData != null)
                {
                    entityData.Value = item.Value.Serialize();
                }
                else
                {
                    entityData = new CategoryData()
                    {
                        Key = key,
                        Value = item.Value.Serialize()
                    };
                }

                output.Add(entityData);
            }

            return output;

        }

        async Task<IList<TCategory>> MergeCategoryData(IList<TCategory> categories)
        {

            if (categories == null)
            {
                return null;
            }

            // Get all category data matching supplied category ids
            var results = await _categoryDataStore.QueryAsync()
                .Select<CategoryDataQueryParams>(q => { q.CategoryId.IsIn(categories.Select(e => e.Id).ToArray()); })
                .ToList();

            if (results == null)
            {
                return categories;
            }

            // Merge data into entities
            return await MergeCategoryData(categories, results.Data);

        }

        async Task<IList<TCategory>> MergeCategoryData(IList<TCategory> categories, IList<CategoryData> data)
        {

            if (categories == null || data == null)
            {
                return categories;
            }

            for (var i = 0; i < categories.Count; i++)
            {
                categories[i].Data = data.Where(d => d.CategoryId == categories[i].Id).ToList();
                categories[i] = await MergeCategoryData(categories[i]);
            }

            return categories;

        }

        async Task<TCategory> MergeCategoryData(TCategory category)
        {

            if (category == null)
            {
                return null;
            }

            if (category.Data == null)
            {
                return category;
            }

            foreach (var data in category.Data)
            {
                var type = await GetModuleTypeCandidateAsync(data.Key);
                if (type != null)
                {
                    var obj = JsonConvert.DeserializeObject(data.Value, type);
                    category.AddOrUpdate(type, (ISerializable) obj);
                }
            }

            return category;

        }

        async Task<Type> GetModuleTypeCandidateAsync(string typeName)
        {
            return await _typedModuleProvider.GetTypeCandidateAsync(typeName, typeof(ISerializable));
        }

        IList<TCategory> PrepareChildren(
            ILookup<int, TCategory> categories,
            IList<TCategory> output = null,
            TCategory parent = null,
            int parentId = 0,
            int depth = 0)
        {

            if (categories == null) throw new ArgumentNullException(nameof(categories));
            if (output == null) output = new List<TCategory>();
            if (parentId == 0) depth = 0;

            foreach (var category in categories[parentId])
            {

                if (depth < 0) depth = 0;
                if (parent != null) depth++;

                category.Depth = depth;
                category.Parent = parent;

                //parent?.Children.Add(category);
                if (parent != null)
                {
                    var children = new List<ICategory>() { category };
                    if (parent.Children != null)
                    {
                        children.AddRange(parent.Children);
                    }
                    parent.Children = children.OrderBy(c => c.SortOrder);;
                }
         
                output.Add(category);

                // recurse
                PrepareChildren(categories, output, category, category.Id, depth--);
            }

            return output;

        }

        IEnumerable<TCategory> RecurseParents(
            IList<TCategory> categories,
            int rootId,
            IList<TCategory> output = null)
        {
            if (output == null)
            {
                output = new List<TCategory>();
            }

            foreach (var category in categories)
            {
                if (category.Id == rootId)
                {
                    if (category.ParentId > 0)
                    {
                        output.Add(category);
                        RecurseParents(categories, category.ParentId, output);
                    }
                    else
                    {
                        output.Add(category);
                    }
                }
            }

            return output;

        }

        IEnumerable<TCategory> RecurseChildren(
            IList<TCategory> categories,
            int rootId,
            IList<TCategory> output = null)
        {

            if (output == null)
            {
                output = new List<TCategory>();
            }

            foreach (var category in categories)
            {
                if (category.ParentId == rootId)
                {
                    output.Add(category);
                    RecurseChildren(categories, category.Id, output);
                }
            }

            return output;

        }

        void CancelTokens(TCategory model)
        {

            // Clear current type
            _cacheManager.CancelTokens(this.GetType());
            _cacheManager.CancelTokens(this.GetType(), ById, model.Id);
            _cacheManager.CancelTokens(this.GetType(), ByFeatureId, model.FeatureId);
            
            // If we instantiate the CategoryStore via a derived type
            // of ICategory i.e. CategoryStore<SomeCategory> ensures we clear
            // the cache for the base category store. We don't want our
            // base category cache polluting our derived type cache
            if (this.GetType() != typeof(CategoryStore<CategoryBase>))
            {
                _cacheManager.CancelTokens(typeof(CategoryStore<CategoryBase>));
                _cacheManager.CancelTokens(typeof(CategoryStore<CategoryBase>), ById, model.Id);
                _cacheManager.CancelTokens(typeof(CategoryStore<CategoryBase>), ByFeatureId, model.FeatureId);
            }

            // Clear category data
            _cacheManager.CancelTokens(typeof(CategoryDataStore));

        }

        #endregion

    }

}
