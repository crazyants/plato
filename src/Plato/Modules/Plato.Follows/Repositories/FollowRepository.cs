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

    class FollowRepository : IFollowRepository<Models.Follow>
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

        public async Task<Models.Follow> InsertUpdateAsync(Models.Follow follow)
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

        public async Task<Models.Follow> SelectByIdAsync(int id)
        {
            Models.Follow follow = null;
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
                            follow = new Models.Follow();
                            follow.PopulateModel(reader);
                        }

                        return follow;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return follow;
        }

        public async Task<IPagedResults<Models.Follow>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<Models.Follow> output = null;
            using (var context = _dbContext)
            {

                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<Models.Follow>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.Follow();
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
                    new IDbDataParameter[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<Models.Follow>> SelectByNameAndThingId(string name, int thingId)
        {
            IList<Models.Follow> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsByNameAndThingId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<Models.Follow>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new Models.Follow();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }

                        }

                        return output;
                    }, new IDbDataParameter[]
                    {
                        new DbParam("Name", DbType.String, 255, name),
                        new DbParam("ThingId", DbType.Int32, thingId)
                    });

            }

            return output;
        }

        public async Task<Models.Follow> SelectByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            Models.Follow follow = null;
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
                            follow = new Models.Follow();
                            follow.PopulateModel(reader);
                        }

                        return follow;

                    }, new IDbDataParameter[]
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
