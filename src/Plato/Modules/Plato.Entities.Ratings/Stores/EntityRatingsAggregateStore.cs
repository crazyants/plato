using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Ratings.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Stores
{

    public class EntityRatingsAggregateStore : IEntityRatingsAggregateStore
    {

        private const string BySql = @"SET NOCOUNT ON 

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
                  
                    -- return updated details
                    SELECT @totalRatings AS TotalRatings,
                            @meanRating AS MeanRating, 
                            @meanRating AS DailyRatings;
	                 
        ";

        private readonly IDbHelper _dbHelper;

        public EntityRatingsAggregateStore(
            IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<AggregateRating> SelectAggregateRating(int entityId)
        {
            return await SelectAggregateRating(entityId, 0);
        }

        public async Task<AggregateRating> SelectAggregateRating(int entityId, int replyId)
        {
            
            // Replacements for SQL script
            var replacements = new Dictionary<string, string>()
            {
                ["{entityId}"] = entityId.ToString(),
                ["{replyId}"] = replyId.ToString()
            };

            return await _dbHelper.ExecuteReaderAsync<AggregateRating>(
                BySql,
                replacements,
                async reader =>
                {
                    var rating = new AggregateRating();
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
