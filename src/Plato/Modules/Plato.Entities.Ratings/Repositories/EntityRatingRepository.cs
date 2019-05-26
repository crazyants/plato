using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Ratings.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Repositories
{
    
    public class EntityRatingRepository : IEntityRatingsRepository<EntityRating>
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityRatingRepository> _logger;

        public EntityRatingRepository(
            IDbContext dbContext,
            ILogger<EntityRatingRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region "Implementation"

        public async Task<EntityRating> InsertUpdateAsync(EntityRating rating)
        {
            if (rating == null)
            {
                throw new ArgumentNullException(nameof(rating));
            }

            var id = await InsertUpdateInternal(
                rating.Id,
                rating.Rating,
                rating.FeatureId,
                rating.EntityId,
                rating.EntityReplyId,
                rating.IpV4Address,
                rating.IpV6Address,
                rating.UserAgent,
                rating.CreatedUserId,
                rating.CreatedDate);

            if (id > 0)
            {
                // return
                return await SelectByIdAsync(id);
            }

            return null;
        }

        public async Task<EntityRating> SelectByIdAsync(int id)
        {
            EntityRating entityRating = null;
            using (var context = _dbContext)
            {
                entityRating = await context.ExecuteReaderAsync2<EntityRating>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingById",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            await reader.ReadAsync();
                            entityRating = new EntityRating();
                            entityRating.PopulateModel(reader);
                        }

                        return entityRating;
                    }, new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });

            }

            return entityRating;

        }

        public async Task<IPagedResults<EntityRating>> SelectAsync(DbParam[] dbParams)
        {
            IPagedResults<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IPagedResults<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new PagedResults<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var rating = new EntityRating();
                                rating.PopulateModel(reader);
                                output.Data.Add(rating);
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
                _logger.LogInformation($"Deleting entity rating with id: {id}");
            }

            var success = 0;
            using (var context = _dbContext)
            {
                success = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "DeleteEntityRatingById",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id)
                    });
            }

            return success > 0 ? true : false;
        }


        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByEntityId(int entityId)
        {

            IList<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IList<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsByEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityRating();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }

            return output;

        }

        public async Task<IEnumerable<EntityRating>> SelectEntityRatingsByUserIdAndEntityId(int userId, int entityId)
        {

            IList<EntityRating> output = null;
            using (var context = _dbContext)
            {
                output = await context.ExecuteReaderAsync2<IList<EntityRating>>(
                    CommandType.StoredProcedure,
                    "SelectEntityRatingsByUserIdAndEntityId",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            output = new List<EntityRating>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityRating();
                                entity.PopulateModel(reader);
                                output.Add(entity);
                            }
                        }

                        return output;
                    }, new[]
                    {
                        new DbParam("UserId", DbType.Int32, userId),
                        new DbParam("EntityId", DbType.Int32, entityId)
                    });

            }

            return output;

        }

        #endregion

        #region "Private Methods"

        async Task<int> InsertUpdateInternal(
            int id,
            int rating,
            int featureId,
            int entityId,
            int entityReplyId,
            string ipV4Address,
            string ipV6Address,
            string userAgent,
            int createdUserId,
            DateTimeOffset? createdDate)
        {

            var emailId = 0;
            using (var context = _dbContext)
            {
                emailId = await context.ExecuteScalarAsync2<int>(
                    CommandType.StoredProcedure,
                    "InsertUpdateEntityRating",
                    new[]
                    {
                        new DbParam("Id", DbType.Int32, id),
                        new DbParam("Rating", DbType.Int32, rating),
                        new DbParam("FeatureId", DbType.Int32, featureId),
                        new DbParam("EntityId", DbType.Int32, entityId),
                        new DbParam("EntityReplyId", DbType.Int32, entityReplyId),
                        new DbParam("IpV4Address", DbType.String, 20, ipV4Address),
                        new DbParam("IpV6Address", DbType.String, 50, ipV6Address),
                        new DbParam("UserAgent", DbType.String, 255, userAgent),
                        new DbParam("CreatedUserId", DbType.Int32, createdUserId),
                        new DbParam("CreatedDate", DbType.DateTimeOffset, createdDate.ToDateIfNull()),
                        new DbParam("UniqueId", DbType.Int32, ParameterDirection.Output),
                    });
            }

            return emailId;

        }

        #endregion

    }

}
