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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelRoleById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    labelRole = new LabelRole();
                    labelRole.PopulateModel(reader);
                }

            }

            return labelRole;

        }

        public async Task<IPagedResults<LabelRole>> SelectAsync(params object[] inputParams)
        {
            PagedResults<LabelRole> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelRolesPaged",
                    inputParams
                );

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

        public async Task<IEnumerable<LabelRole>> SelectByLabelIdAsync(int LabelId)
        {

            List<LabelRole> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectLabelRolesByLabelId",
                    LabelId
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<LabelRole>();
                    while (await reader.ReadAsync())
                    {
                        var Label = new LabelRole();
                        Label.PopulateModel(reader);
                        output.Add(Label);
                    }

                }
            }

            return output;
        }

        public async Task<bool> DeleteByLabelIdAsync(int LabelId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting Label roles for Label Id: {LabelId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelRolesByLabelId", LabelId);
            }

            return success > 0 ? true : false;

        }

        public async Task<bool> DeleteByRoleIdAndLabelIdAsync(int roleId, int LabelId)
        {

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting Label roles for Label Id: {LabelId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteLabelRolesByRoleIdAndLabelId",
                    roleId,
                    LabelId);
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
            DateTime? createdDate,
            int modifiedUserId,
            DateTime? modifiedDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateLabelRole",
                    id,
                    labelId,
                    roleId,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull(),
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion

    }

}
