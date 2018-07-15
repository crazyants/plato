using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Categories.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Categories.Repositories
{

    public class CategoryRepository : ICategoryRepository<Category>
    {
     
        #region "Constructor"

        private readonly ICategoryDataRepository<CategoryData> _categoryDataRepository;

        private readonly IDbContext _dbContext;
        private readonly ILogger<CategoryRepository> _logger;
      
        public CategoryRepository(
            IDbContext dbContext,
            ILogger<CategoryRepository> logger,
            ICategoryDataRepository<CategoryData> categoryDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _categoryDataRepository = categoryDataRepository;
        }

        #endregion
        
        #region "Implementation"

        public async Task<Category> InsertUpdateAsync(Category model)
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
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Category> SelectByIdAsync(int id)
        {

            Category category = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoryById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    category = new Category();
                    category.PopulateModel(reader);
                }

            }

            return category;

        }
        
        public async Task<IPagedResults<Category>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Category> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoriesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<Category>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new Category();
                        entity.PopulateModel(reader);
                        output.Data.Add(entity);
                    }

                    if (await reader.NextResultAsync())
                    {
                        await reader.ReadAsync();
                        output.PopulateTotal(reader);
                    }
                    
                }
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
                    "DeleteCategoryById", id);
            }

            return success > 0 ? true : false;

        }
        
        public async Task<IEnumerable<Category>> SelectByFeatureIdAsync(int featureId)
        {

            List<Category> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectCategoriesByFeatureId",
                    featureId
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<Category>();
                    while (await reader.ReadAsync())
                    {
                        var category = new Category();
                        category.PopulateModel(reader);
                        output.Add(category);
                    }
                    
                }
            }

            return output;
        }

        #endregion

        #region "Private Methods"

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
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate,
            IEnumerable<CategoryData> data)
        {

            var categoryId = 0;
            using (var context = _dbContext)
            {

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for category with name '{name}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for category with Id {id} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

                categoryId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateCategory",
                    id,
                    parentId,
                    featureId,
                    name.ToEmptyIfNull().TrimToSize(255),
                    description.ToEmptyIfNull().TrimToSize(500),
                    alias.ToEmptyIfNull().TrimToSize(255),
                    iconCss.ToEmptyIfNull().TrimToSize(50),
                    foreColor.ToEmptyIfNull().TrimToSize(50),
                    backColor.ToEmptyIfNull().TrimToSize(50),
                    sortOrder,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull());
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