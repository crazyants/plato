using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Follows.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Follows.Repositories
{

    class FollowRepository : IFollowRepository<Follow>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<FollowRepository> _logger;

        public FollowRepository(
            IDbContext dbContext,
            ILogger<FollowRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        #region "Implementation"

        public async Task<Follow> InsertUpdateAsync(Follow follow)
        {
            if (follow == null)
            {
                throw new ArgumentNullException(nameof(follow));
            }
                
            var id = await InsertUpdateInternal(
                follow.Id,
                follow.Name,
                follow.ThingId,
                follow.CancellationToken,
                follow.CreatedUserId,
                follow.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<Follow> SelectByIdAsync(int id)
        {
            Follow follow = null;
            using (var context = _dbContext)
            {
                follow = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            follow = new Follow();
                            follow.PopulateModel(reader);
                        }

                        return follow;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return follow;
        }

        public async Task<IPagedResults<Follow>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<Follow> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Follow>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Follow();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
                            }

                            if (await reader.NextResultAsync())
                            {
                                if (reader.HasRows)
                                {
                                    await reader.ReadAsync();
                                    output.PopulateTotal(reader);
                                }
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
                _logger.LogInformation($"Deleting entity with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "DeleteFollowById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<Follow>> SelectByNameAndThingId(string name, int thingId)
        {
            IList<Follow> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsByNameAndThingId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<Follow>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Follow();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("ThingId", DbType.Int32, thingId)
                    });

            }

            return output;
        }

        public async Task<Follow> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            Follow follow = null;
            using (var context = _dbContext)
            {
                follow = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowByNameThingIdAndCreatedUserId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            follow = new Follow();
                            follow.PopulateModel(reader);
                        }

                        return follow;

                    }, new[]
                    {
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("ThingId", DbType.Int32, thingId),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                    });
                
            }

            return follow;
        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            string name,
            int thingId,
            string cancellationToken,
            int createdUserId,
            DateTimeOffset createdDate)
        {

            var output = 0;
            using (var context = _dbContext)
            {
                output = await context.ExecuteScalarAsync<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateFollow",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("ThingId", DbType.Int32, thingId),
                        new DbParam("CancellationToken", DbType.String, 100, cancellationToken),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output)
                    });
            }

            return output;

        }

        #endregion
        
    }
}
