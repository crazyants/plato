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

    public class LabelRoleRepository : ILabelRoleRepository<LabelRole>
    {

        #region "Constructor"

        private readonly ILabelDataRepository<LabelData> _LabelDataRepository;

        private readonly IDbContext _dbContext;
        private readonly ILogger<LabelRoleRepository> _logger;

        public LabelRoleRepository(
            IDbContext dbContext,
            ILogger<LabelRoleRepository> logger,
            ILabelDataRepository<LabelData> LabelDataRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _LabelDataRepository = LabelDataRepository;
        }

        #endregion

        #region "Implementation"

        public async Task<LabelRole> InsertUpdateAsync(LabelRole model)
        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var id = await InsertUpdateInternal(
                model.Id,
                model.LabelId,
                model.RoleId,
                model.CreatedUserId,
                model.CreatedDate,
                model.ModifiedUserId,
                model.ModifiedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<LabelRole> SelectByIdAsync(int id)
        {

            LabelRole labelRole = null;
            using (var context = _dbContext)
            {
                labelRole = await context.ExecuteReaderAsync<LabelRole>(
                    CommandType.StoredProcedure,
                    "SelectLabelRoleById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            labelRole = new LabelRole();
                            labelRole.PopulateModel(reader);
                        }

                        return labelRole;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });


            }

            return labelRole;

        }

        public async Task<IPagedResults<LabelRole>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<LabelRole> output = null;
            using (var context = _dbContext)
            {
                
                output = await context.ExecuteReaderAsync<IPagedResults<LabelRole>>(
                    CommandType.StoredProcedure,
                    "SelectLabelRolesPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<LabelRole>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new LabelRole();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                        }

                        return output;

                    },
                    dbParams
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
                    "DeleteLabelById", new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<LabelRole>> SelectByLabelIdAsync(int labelId)
        {

            List<LabelRole> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelRolesByLabelId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<LabelRole>();
                            while (await reader.ReadAsync())
                            {
                                var labelRole = new LabelRole();
                                labelRole.PopulateModel(reader);
                                output.Add(labelRole);
                            }

                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("LabelId", DbType.Int32, labelId)
                    });

            }

            return output;
        }

        public async Task<bool> DeleteByLabelIdAsync(int labelId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting Label roles for Label Id: {labelId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelRolesByLabelId",
                    new []
                    {
                        new DbParam("LabelId", DbType.Int32, labelId)
                    });
            }

            return success > 0 ? true : false;

        }

        public async Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int labelId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting Label roles for Label Id: {labelId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelRolesByRoleIdAndLabelId",
                    new[]
                    {
                        new DbParam("RoleId", DbType.Int32, roleId),
                        new DbParam("LabelId", DbType.Int32, labelId),
                    });
            }

            return success > 0 ? true : false;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int labelId,
            int roleId,
            int createdUserId,
            DateTimeOffset? createdDate,
            int modifiedUserId,
            DateTimeOffset? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateLabelRole",
                    new []
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("LabelId", DbType.Int32, labelId),
                        new DbParam("RoleId", DbType.Int32, roleId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("ModifiedUserId", DbType.Int32, modifiedUserId),
                        new DbParam("ModifiedDate", DbType.DateTimeOffset, modifiedDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion

    }

}
