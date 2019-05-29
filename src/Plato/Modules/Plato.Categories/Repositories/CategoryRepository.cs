using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.Repositories
{

    public class CategoryRepository<TCategory> : ICategoryRepository<TCategory> where TCategory : class, ICategory
    {

        #region "Constructor"

        private readonly ICategoryDataRepository<CategoryData> _categoryDataRepository;
        private readonly IDbContext _dbContext;
        private readonly ILogger<CategoryRepository<TCategory>> _logger;

        public CategoryRepository(
            IDbContext dbContext,
            ILogger<CategoryRepository<TCategory>> logger,
            ICategoryDataRepository<CategoryData> categoryDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _categoryDataRepository = categoryDataRepository;
        }

        #endregion

        #region "Implementation"

        public async Task<TCategory> InsertUpdateAsync(TCategory model)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var id = await InsertUpdateInternal(
                model.Id,
                model.ParentId,
                model.FeatureId,
                model.Name,
                model.Description,
                model.Alias,
                model.IconCss,
                model.ForeColor,
                model.BackColor,
                model.SortOrder,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate,
                model.Data);
            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<TCategory> SelectByIdAsync(int id)
        {

            TCategory category = null;
            using (var context = _dbContext)
            {
                category = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryById",
                    async reader => await BuildCategoryFromResultSets(reader),
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return category;

        }

        public async Task<IPagedResults<TCategory>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<TCategory> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<TCategory>>(
                    CommandType.StoredProcedure,
                    "SelectCategoriesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<TCategory>();
                            while (await reader.ReadAsync())
                            {
                                var category = ActivateInstanceOf<TCategory>.Instance();
                                category.PopulateModel(reader);
                                output.Data.Add(category);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
                            }

                        }

                        return output;
                    },
                    dbParams);

            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting category with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteCategoryById",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<TCategory>> SelectByFeatureIdAsync(int featureId)
        {

            IList<TCategory> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<TCategory>>(
                    CommandType.StoredProcedure,
                    "SelectCategoriesByFeatureId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<TCategory>();
                            while (await reader.ReadAsync())
                            {
                                var category = ActivateInstanceOf<TCategory>.Instance();
                                category.PopulateModel(reader);
                                output.Add(category);
                            }
                        }

                        return output;

                    }, new IDbDataParameter[]
                    {
                        new DbParam("FeatureId", DbType.Int32, featureId)
                    });

            }

            return output;

        }

        #endregion

        #region "Private Methods"

        async Task<TCategory> BuildCategoryFromResultSets(DbDataReader reader)
        {

            TCategory model = null;
            if ((reader != null) && (reader.HasRows))
            {

                // Category
                model = ActivateInstanceOf<TCategory>.Instance();

                await reader.ReadAsync();
                model.PopulateModel(reader);
               
                // Data
                if (await reader.NextResultAsync())
                {
                    if (reader.HasRows)
                    {
                        var data = new List<CategoryData>();
                        while (await reader.ReadAsync())
                        {
                            data.Add(new CategoryData(reader));
                        }
                        model.Data = data;
                    }

                }

            }

            return model;
        }


        async Task<int> InsertUpdateInternal(
            int id,
            int parentId,
            int featureId,
            string name,
            string description,
            string alias,
            string iconCss,
            string foreColor,
            string backColor,
            int sortOrder,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate,
            IEnumerable<CategoryData> data)
        {

            var categoryId = 0;
            using (var context = _dbContext)
            {
                categoryId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategory",
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ParentId", DbType.Int32, parentId),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("Name", DbType.String, 255, name.ToEmptyIfNull()),
                        new DbParam("Description", DbType.String, 500, description.ToEmptyIfNull()),
                        new DbParam("Alias", DbType.String, 255, alias.ToEmptyIfNull()),
                        new DbParam("IconCss", DbType.String, 50, iconCss.ToEmptyIfNull()),
                        new DbParam("ForeColor", DbType.String, 50, foreColor.ToEmptyIfNull()),
                        new DbParam("BackColor", DbType.String, 50, backColor.ToEmptyIfNull()),
                        new DbParam("SortOrder", DbType.Int32, sortOrder),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            // Add category data
            if (categoryId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.CategoryId = categoryId;
                        await _categoryDataRepository.InsertUpdateAsync(item);
                    }
                }

            }

            return categoryId;

        }

        #endregion

    }
    
}