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
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowById", id);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    follow = new Follow();
                    follow.PopulateModel(reader);
                }

            }

            return follow;
        }

        public async Task<IPagedResults<Follow>> SelectAsync(params object[] inputParams)
        {
            PagedResults<Follow> output = null;
            using (var context = _dbContext)
            {

                _dbContext.OnException += (sender, args) =>
                {
                    if (_logger.IsEnabled(LogLevel.Error))
                        _logger.LogInformation($"SelectEntitiesPaged failed with the following error {args.Exception.Message}");
                };

                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsPaged",
                    inputParams
                );

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
                    "DeleteFollowById", id);
            }

            return success > 0 ? true : false;
        }

        public async Task<IEnumerable<Follow>> SelectFollowsByNameAndThingId(string name, int thingId)
        {
            List<Follow> output = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowsByNameAndThingId",
                    name,
                    thingId);
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
            }

            return output;
        }

        public async Task<Follow> SelectFollowByNameThingIdAndCreatedUserId(string name, int thingId, int createdUserId)
        {
            Follow follow = null;
            using (var context = _dbContext)
            {
                var reader = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectFollowByNameThingIdAndCreatedUserId", 
                    name,
                    thingId,
                    createdUserId);
                if ((reader != null) && (reader.HasRows))
                {
                    await reader.ReadAsync();
                    follow = new Follow();
                    follow.PopulateModel(reader);
                }

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
                    id,
                    name.ToEmptyIfNull().TrimToSize(255),
                    thingId,
                    cancellationToken.ToEmptyIfNull().TrimToSize(100),
                    createdUserId,
                    createdDate,
                    new DbDataParameter(DbType.Int32, ParameterDirection.Output));
            }

            return output;

        }

        #endregion
        
    }
}
