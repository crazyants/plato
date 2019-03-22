using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Entities.Ratings.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Services
{

    public class EntityRatingsUpdater : IEntityRatingsUpdater
    {

        private const string ByEntitySql = @"SET NOCOUNT ON 

                    -- get total & summed ratings

                    DECLARE @totalRatings int, 
		                    @summedRatings int;

                    SET @totalRatings = (
	                    SELECT COUNT(Id) FROM {prefix}_EntityRatings WITH (nolock) WHERE (EntityId = {entityId} AND EntityReplyId = {replyId})
                    );
                    SET @summedRatings =  (
	                    SELECT SUM(Rating) FROM {prefix}_EntityRatings WITH (nolock) WHERE (EntityId = {entityId} AND EntityReplyId = {replyId})
                    );

                    -- accomodate for nulls within our counts
                    SET @totalRatings = IsNull(@totalRatings, 0)
                    SET @summedRatings = IsNull(@summedRatings, 0)

                    -- calculate mean rating

                    DECLARE @meanRating int
                    IF(@totalRatings > 0)
                    BEGIN
	                    SET @meanRating = (@summedRatings / @totalRatings)
                    END
                    ELSE
                    BEGIN
	                    SET @meanRating = 0
                    END

                    -- calculate daily ratings

                    DECLARE @createdDate DateTime2;
                    SET @createdDate = (
	                    SELECT CreatedDate FROM {prefix}_Entities WHERE (Id = {entityId})
                    );

                    DECLARE @dayDiff int = 0, 
		                    @dailyRatings int = 0

                    IF (NOT @createdDate IS NULL)
                    BEGIN	
	                    SET @dayDiff = (SELECT DATEDIFF(day, @createdDate, GetDate()));
	                    if (@dayDiff > 0)
	                    BEGIN
		                    SET @dayDiff = (@totalRatings / @dayDiff);
	                    END
                    END

                    -- update entity
                    UPDATE {prefix}_Entities SET 
	                    TotalRatings = @totalRatings, 
	                    MeanRating = @meanRating,
	                    DailyRatings = @dailyRatings
                    WHERE (Id = {entityId});

                    -- return updated details
                    SELECT TotalRatings, MeanRating, DailyRatings FROM 
	                    {prefix}_Entities
                    WHERE (Id = {entityId});
        ";

        private const string ByReplySql = @"SET NOCOUNT ON 

                    -- get total & summed ratings

                    DECLARE @totalRatings int, 
		                    @summedRatings int;

                    SET @totalRatings = (
	                    SELECT COUNT(Id) FROM {prefix}_EntityRatings WITH (nolock) WHERE (EntityId = {entityId} AND EntityReplyId = {replyId})
                    );
                    SET @summedRatings =  (
	                    SELECT SUM(Rating) FROM {prefix}_EntityRatings WITH (nolock) WHERE (EntityId = {entityId} AND EntityReplyId = {replyId})
                    );

                    -- accomodate for nulls within our counts
                    SET @totalRatings = IsNull(@totalRatings, 0)
                    SET @summedRatings = IsNull(@summedRatings, 0)

                    -- calculate mean rating

                    DECLARE @meanRating int
                    IF(@totalRatings > 0)
                    BEGIN
	                    SET @meanRating = (@summedRatings / @totalRatings)
                    END
                    ELSE
                    BEGIN
	                    SET @meanRating = 0
                    END
                    
                    -- update entity replies
                    UPDATE {prefix}_EntityReplies SET 
	                    TotalRatings = @totalRatings, 
	                    MeanRating = @meanRating
                    WHERE (Id = {entityId});

                    -- return updated details
                    SELECT TotalRatings, MeanRating, 0 AS DailyRatings FROM 
	                    {prefix}_EntityReplies
                    WHERE (Id = {entityId});
            ";


        private readonly IDbHelper _dbHelper;

        public EntityRatingsUpdater(
            IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<UpdatedRating> UpdateEntityRating(int entityId)
        {
            return await UpdateEntityRating(entityId, 0);
        }

        public async Task<UpdatedRating> UpdateEntityRating(int entityId, int replyId)
        {
            
            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{entityId}"] = entityId.ToString(),
                ["{replyId}"] = replyId.ToString()
            };

            return await _dbHelper.ExecuteReaderAsync<UpdatedRating>(
                replyId == 0 ? ByEntitySql : ByReplySql,
                replacements,
                async reader =>
                {
                    var rating = new UpdatedRating();
                    while (await reader.ReadAsync())
                    {
                        if (reader.ColumnIsNotNull("TotalRatings"))
                        {
                            rating.TotalRatings = Convert.ToInt32(reader["TotalRatings"]);
                        }

                        if (reader.ColumnIsNotNull("MeanRating"))
                        {
                            rating.MeanRating = Convert.ToInt32(reader["MeanRating"]);
                        }

                        if (reader.ColumnIsNotNull("DailyRatings"))
                        {
                            rating.DailyRatings = Convert.ToInt32(reader["DailyRatings"]);
                        }

                    }

                    return rating;

                });

        }
    }

}
