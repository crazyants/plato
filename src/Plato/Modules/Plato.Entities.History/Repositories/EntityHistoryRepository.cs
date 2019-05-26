using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.History.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.History.Repositories
{

    public class EntityHistoryRepository : IEntityHistoryRepository<EntityHistory>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityHistoryRepository> _logger;

        public EntityHistoryRepository(
            IDbContext dbContext,
            ILogger<EntityHistoryRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<EntityHistory> InsertUpdateAsync(EntityHistory history)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }
                
            var id = await InsertUpdateInternal(
                history.Id,
                history.EntityId,
                history.EntityReplyId,
                history.Message,
                history.Html,
                history.MajorVersion,
                history.MinorVersion,
                history.CreatedUserId,
                history.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityHistory> SelectByIdAsync(int id)
        {

            EntityHistory history = null;
            using (var context = _dbContext)
            {
                history = await context.ExecuteReaderAsync2(
                    CommandType.StoredProcedure,
                    "SelectEntityHistoryById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            history = new EntityHistory();
                            history.PopulateModel(reader);
                        }

                        return history;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return history;

        }

        public async Task<IPagedResults<EntityHistory>> SelectAsync(DbParam[] dbParams)
        {
            IPagedResults<EntityHistory> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync2<IPagedResults<EntityHistory>>(
                    CommandType.StoredProcedure,
                    "SelectEntityHistoryPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityHistory>();
                            while (await reader.ReadAsync())
                            {
                                var history = new EntityHistory();
                                history.PopulateModel(reader);
                                output.Data.Add(history);
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

        public async Task<bool> DeleteAsync(int id)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Deleting entity history with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityHistoryById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

     
        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int entityId,
            int entityReplyId,
            string message,
            string html,
            short majorVersion,
            short minorVersion,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityHistory",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId),
                        new DbParam("Message", DbType.String, message),
                        new DbParam("Html", DbType.String, html),
                        new DbParam("MajorVersion", DbType.Int16, majorVersion),
                        new DbParam("MinorVersion", DbType.Int16, minorVersion),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return output;

        }

        #endregion
        
    }
}
