using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Models;

namespace Plato.Labels.Repositories
{

    public class LabelRepository<TLabel> : ILabelRepository<TLabel> where TLabel : class, ILabel
    {

        #region "Constructor"

        private readonly ILabelDataRepository<LabelData> _LabelDataRepository;
        private readonly IDbContext _dbContext;
        private readonly ILogger<LabelRepository<TLabel>> _logger;

        public LabelRepository(
            IDbContext dbContext,
            ILogger<LabelRepository<TLabel>> logger,
            ILabelDataRepository<LabelData> labelDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _LabelDataRepository = labelDataRepository;
        }

        #endregion

        #region "Implementation"

        public async Task<TLabel> InsertUpdateAsync(TLabel model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
                
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
                model.TotalEntities,
                model.TotalFollows,
                model.TotalViews,
                model.LastEntityId,
                model.LastEntityDate,
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

        public async Task<TLabel> SelectByIdAsync(int id)
        {

            TLabel label = null;
            using (var context = _dbContext)
            {
                label = await context.ExecuteReaderAsync2<TLabel>(
                    CommandType.StoredProcedure,
                    "SelectLabelById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            label = ActivateInstanceOf<TLabel>.Instance();
                            label.PopulateModel(reader);
                        }

                        return label;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return label;

        }

        public async Task<IPagedResults<TLabel>> SelectAsync(params object[] inputParams)
        {
            IPagedResults<TLabel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<TLabel>>(
                    CommandType.StoredProcedure,
                    "SelectLabelsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<TLabel>();
                            while (await reader.ReadAsync())
                            {
                                var label = ActivateInstanceOf<TLabel>.Instance();
                                label.PopulateModel(reader);
                                output.Data.Add(label);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;
                    },
                    inputParams);

            }

            return output;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting Label with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<TLabel>> SelectByFeatureIdAsync(int featureId)
        {

            IList<TLabel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IList<TLabel>>(
                    CommandType.StoredProcedure,
                    "SelectLabelsByFeatureId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<TLabel>();
                            while (await reader.ReadAsync())
                            {
                                var label = ActivateInstanceOf<TLabel>.Instance();
                                label.PopulateModel(reader);
                                output.Add(label);
                            }

                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("FeatureId", DbType.Int32, featureId)
                    });

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
            int totalEntities,
            int totalFollows,
            int totalViews,
            int lastEntityId,
            DateTimeOffset? lastEntityDate,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate,
            IEnumerable<LabelData> data)
        {

            var labelId = 0;
            using (var context = _dbContext)
            {
                labelId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateLabel",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("ParentId", DbType.Int32, parentId),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("Description", DbType.String, 500, description),
                        new DbParam("Alias", DbType.String, 255, alias),
                        new DbParam("IconCss", DbType.String, 50, iconCss.ToEmptyIfNull()),
                        new DbParam("ForeColor", DbType.String, 50, foreColor),
                        new DbParam("BackColor", DbType.String, 50, backColor),
                        new DbParam("SortOrder", DbType.Int32, sortOrder),
                        new DbParam("TotalEntities", DbType.Int32, totalEntities),
                        new DbParam("TotalFollows", DbType.Int32, totalFollows),
                        new DbParam("TotalViews", DbType.Int32, totalViews),
                        new DbParam("LastEntityId", DbType.Int32, lastEntityId),
                        new DbParam("LastEntityDate", DbType.DateTimeOffset, lastEntityDate),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            // Add Label data
            if (labelId > 0)
            {
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        item.LabelId = labelId;
                        await _LabelDataRepository.InsertUpdateAsync(item);
                    }
                }

            }

            return labelId;

        }

        #endregion

    }
    
}