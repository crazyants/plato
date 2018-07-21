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

    public class EntityLabelRepository : IEntityLabelRepository<EntityLabel>
    {
        
        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityLabelRepository> _logger;

        public EntityLabelRepository(
            IDbContext dbContext,
            ILogger<EntityLabelRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityLabel> InsertUpdateAsync(EntityLabel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var id = await InsertUpdateInternal(
                model.Id,
                model.EntityId,
                model.LabelId,
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

        public async Task<EntityLabel> SelectByIdAsync(int id)
        {
            EntityLabel output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityLabelById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    output = new EntityLabel();
                    output.PopulateModel(reader);
                }

            }

            return output;

        }

        public async Task<IPagedResults<EntityLabel>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityLabel> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityLabel>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityLabel();
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
                _logger.LogInformation($"Deleting entity Label relationship with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityLabelById", id);
            }

            return success > 0 ? true : false;

        }

        public async Task<IEnumerable<EntityLabel>> SelectByEntityId(int entityId)
        {
            List<EntityLabel> output = null;
            using (var context = _dbContext)
            {

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityCategoriesByEntityId",
                    entityId);

                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<EntityLabel>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityLabel();
                        entity.PopulateModel(reader);
                        output.Add(entity);
                    }
                    
                }
            }

            return output;
        }

        public async Task<bool> DeleteByEntityId(int entityId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting all entity Label relationships for entity id: {entityId}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityCategoriesByEntityId", entityId);
            }

            return success > 0 ? true : false;
        }

        public async Task<bool> DeleteByEntityIdAndLabelId(int entityId, int LabelId)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity Label relationship with entityId '{entityId}' and LabelId '{LabelId}'");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityLabelByEntityIdAndLabelId",
                    entityId,
                    LabelId);
            }

            return success > 0 ? true : false;
        }

        #endregion

        #region "Private Methods"


        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int LabelId,
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
                    "InsertUpdateEntityLabel",
                    id,
                    entityId,
                    LabelId,
                    createdUserId,
                    createdDate.ToDateIfNull(),
                    modifiedUserId,
                    modifiedDate.ToDateIfNull());
            }

            return output;

        }
        
        #endregion

    }
}
