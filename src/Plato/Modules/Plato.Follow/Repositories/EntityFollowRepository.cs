using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Follow.Models;

namespace Plato.Follow.Repositories
{
    
    class EntityFollowRepository : IEntityFollowRepository<EntityFollow>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityFollowRepository> _logger;

        public EntityFollowRepository(
            IDbContext dbContext,
            ILogger<EntityFollowRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<EntityFollow> InsertUpdateAsync(EntityFollow follow)
        {
            if (follow == null)
            {
                throw new ArgumentNullException(nameof(follow));
            }
                
            var id = await InsertUpdateInternal(
                follow.Id,
                follow.EntityId,
                follow.UserId,
                follow.CancellationGuid,
                follow.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityFollow> SelectByIdAsync(int id)
        {
            EntityFollow follow = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityFollowById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    follow = new EntityFollow();
                    follow.PopulateModel(reader);
                }

            }

            return follow;
        }

        public async Task<IPagedResults<EntityFollow>> SelectAsync(params object[] inputParams)
        {
            PagedResults<EntityFollow> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityFollowsPaged",
                    inputParams
                );

                if ((reader != null) && (reader.HasRows))
                {
                    output = new PagedResults<EntityFollow>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityFollow();
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
                _logger.LogInformation($"Deleting entity with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityFollowById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<EntityFollow>> SelectEntityFollowsByEntityId(int entityId)
        {
            List<EntityFollow> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityFollowsByEntityId",
                    entityId);
                if ((reader != null) && (reader.HasRows))
                {
                    output = new List<EntityFollow>();
                    while (await reader.ReadAsync())
                    {
                        var entity = new EntityFollow();
                        entity.PopulateModel(reader);
                        output.Add(entity);
                    }
                    
                }
            }

            return output;
        }

        public async Task<EntityFollow> SelectEntityFollowByUserIdAndEntityId(int userId, int entityId)
        {
            EntityFollow follow = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntityFollowsByUserIdAndEntityId", 
                    userId,
                    entityId);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    follow = new EntityFollow();
                    follow.PopulateModel(reader);
                }

            }

            return follow;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int userId,
            string cancellationGuid,
            DateTimeOffset createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityFollow",
                    id,
                    entityId,
                    userId,
                    cancellationGuid.ToEmptyIfNull().TrimToSize(100),
                    createdDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion
        
    }
}
