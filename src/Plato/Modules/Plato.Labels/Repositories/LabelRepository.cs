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
                label = await context.ExecuteReaderAsync<TLabel>(
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
                    },
                    id);
             
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
                    inputParams
                );

            
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
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<TLabel>> SelectByFeatureIdAsync(int featureId)
        {

            IList<TLabel> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IList<TLabel>>(
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
                    },
                    featureId);

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

                context.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation(
                            id == 0
                                ? $"Insert for Label with name '{name}' failed with the following error '{args.Exception.Message}'"
                                : $"Update for Label with Id {id} failed with the following error {args.Exception.Message}");
                    throw args.Exception;
                };

                labelId = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateLabel",
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
                    totalEntities,
                    totalFollows,
                    totalViews,
                    lastEntityId,
                    lastEntityDate,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
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