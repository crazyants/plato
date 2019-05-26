using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Models;

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
            LabelData data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelDatumById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            data = new LabelData();
                            await reader.ReadAsync();
                            data.PopulateModel(reader);
                        }

                        return data;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return data;

        }

        public async Task<IEnumerable<LabelData>> SelectByLabelIdAsync(int labelId)
        {
            IList<LabelData> data = null;
            using (var context = _dbContext)
            {
                data = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelDatumByLabelId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            data = new List<LabelData>();
                            while (await reader.ReadAsync())
                            {
                                var entityData = new LabelData();
                                entityData.PopulateModel(reader);
                                data.Add(entityData);
                            }
                        }

                        return data;
                    }, new[]
                    {
                        new DbParam("LabelId", DbType.Int32, labelId)
                    });

            }
            return data;

        }

        public async Task<LabelData> InsertUpdateAsync(LabelData data)
        {
            var id = await InsertUpdateInternal(
                data.Id,
                data.LabelId,
                data.Key,
                data.Value,
                data.CreatedDate,
                data.CreatedUserId,
                data.ModifiedDate,
                data.ModifiedUserId);
            if (id > 0)
            {
                return await SelectByIdAsync(id);
            }
                
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
                    "DeleteLabelDatumById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IPagedResults<LabelData>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<LabelData> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync<IPagedResults<LabelData>>(
                    CommandType.StoredProcedure,
                    "SelectLabelDatumPaged",
                    async reader =>
                    {
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

                        return output;
                    },
                    dbParams);

            }

            return output;
        }

        #endregion

        #region "Private Methods"

        private async Task<int> InsertUpdateInternal(
            int id,
            int labelId,
            string key,
            string value,
            DateTimeOffset? createdDate,
            int createdUserId,
            DateTimeOffset? modifiedDate,
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
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("LabelId", DbType.Int32, labelId),
                        new DbParam("Key", DbType.String, 255, key),
                        new DbParam("Value", DbType.String, value),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

        }

        #endregion

    }



}
