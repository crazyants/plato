using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;

namespace Plato.Internal.Repositories.Metrics
{
    
    public class AggregatedUserRepository : IAggregatedUserRepository
    {
      
        private readonly IDbHelper _dbHelper;

        public AggregatedUserRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end)
        {

            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_Users 
                WHERE 
                    {groupBy} >= '{start}' AND {groupBy} <= '{end}'
                GROUP BY 
                    YEAR({groupBy}),
                    MONTH({groupBy}), 
                    DAY({groupBy})
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<DateTimeOffset>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<DateTimeOffset>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });
            
        }

        public async Task<AggregatedResult<string>> SelectUserMetricsAsync(DateTimeOffset start, DateTimeOffset end)
        {

            // Sql query
            const string sql = @"                                
              
                DECLARE @totalUsers int;
                DECLARE @returningUsers int;
                DECLARE @newUsers int;
                DECLARE @totalBadges int;
                DECLARE @newBadges int;
                DECLARE @totalReputations int;
                DECLARE @newReputations int;
                    
                SET @totalUsers = (SELECT COUNT(Id) FROM {prefix}_Users);
                
                SET @newUsers = (
	                SELECT COUNT(Id) FROM {prefix}_Users
	                WHERE CreatedDate >= '{start}' AND CreatedDate <= '{end}'
                );

                SET @returningUsers = (
	                SELECT COUNT(Id) FROM {prefix}_Users
	                WHERE CreatedDate <= '{start}' AND Id IN (
                         SELECT DISTINCT CreatedUserId FROM {prefix}_UserReputations 	
	                    WHERE ([Name] = 'Visit') AND (CreatedDate >= '{start}' AND CreatedDate <= '{end}')
                    )
                );

                SET @totalBadges = (
	                SELECT COUNT(Id) FROM {prefix}_UserBadges
                );

               SET @newBadges = (
	                SELECT COUNT(Id) FROM {prefix}_UserBadges 
	                WHERE CreatedDate >= '{start}' AND CreatedDate <= '{end}'
                );

                SET @totalReputations = (
	                SELECT COUNT(Id) FROM {prefix}_UserReputations
                );

                SET @newReputations = (
	                SELECT COUNT(Id) FROM {prefix}_UserReputations 	
	                WHERE CreatedDate >= '{start}' AND CreatedDate <= '{end}'
                );

                DECLARE @temp TABLE
                (
	                [Aggregate] nvarchar(100) NOT NULL,
	                [Count] int NOT NULL
                );

                INSERT INTO @temp SELECT 'TotalUsers', @totalUsers;
                INSERT INTO @temp SELECT 'NewUsers', @newUsers;
                INSERT INTO @temp SELECT 'ReturningUsers', @returningUsers;
                INSERT INTO @temp SELECT 'TotalBadges', @totalBadges;
                INSERT INTO @temp SELECT 'NewBadges', @newBadges;
                INSERT INTO @temp SELECT 'TotalReputations', @totalReputations;
                INSERT INTO @temp SELECT 'NewReputations', @newReputations;             

                SELECT * FROM @temp;
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern()
            };

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<string>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<string>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });


        }


    }

}
