using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Labels.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Labels.Repositories
{

    public class LabelDataRepository : ILabelDataRepository<LabelData>
    {

        #region Private Variables"

        private readonly IDbContext _dbContext;
        private readonly ILogger<LabelDataRepository> _logger;

        #endregion

        #region "Constructor"

        public LabelDataRepository(
            IDbContext dbContext,
            ILogger<LabelDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public async Task<LabelData> SelectByIdAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting entity data with id: {id}");
            }

            LabelData data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                  CommandType.StoredProcedure,
                    "SelectLabelDatumById", id);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new LabelData();
                        await reader.ReadAsync();
                        data.PopulateModel(reader);
                    }
                }

            }

            return data;

        }

        public async Task<IEnumerable<LabelData>> SelectByLabelIdAsync(int LabelId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Selecting all Label data for id {LabelId}");
            }

            List<LabelData> data = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelDatumByLabelId",
                    LabelId);
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        data = new List<LabelData>();
                        while (await reader.ReadAsync())
                        {
                            var entityData = new LabelData();
                            entityData.PopulateModel(reader);
                            data.Add(entityData);
                        }
                    }
                }
            }
            return data;

        }

        public async Task<LabelData> InsertUpdateAsync(LabelData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.LabelId,
                data.Key.ToEmptyIfNull().TrimToSize(255),
                data.Value.ToEmptyIfNull(),
                data.CreatedDate.ToDateIfNull(),
                data.CreatedUserId,
                data.ModifiedDate.ToDateIfNull(),
                data.ModifiedUserId);
            if (id > 0)
                return await SelectByIdAsync(id);
            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity data id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelDatumById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IPagedResults<LabelData>> SelectAsync(params object[] inputParams)
        {
            PagedResults<LabelData> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelDatumPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<LabelData>();
                    while (await reader.ReadAsync())
                    {
                        var data = new LabelData();
                        data.PopulateModel(reader);
                        output.Data.Add(data);
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

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int LabelId,
            string key,
            string value,
            DateTime? createdDate,
            int createdUserId,
            DateTime? modifiedDate,
            int modifiedUserId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(id == 0
                    ? $"Inserting Label data with key: {key}"
                    : $"Updating Label data with id: {id}");
            }

            using (var context = _dbContext)
            {
                if (context == null)
                    return 0;
                return await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateLabelDatum",
                    id,
                    LabelId,
                    key.ToEmptyIfNull().TrimToSize(255),
                    value.ToEmptyIfNull(),
                    createdDate.ToDateIfNull(),
                    createdUserId,
                    modifiedDate.ToDateIfNull(),
                    modifiedUserId);
            }

        }

        #endregion

    }



}
