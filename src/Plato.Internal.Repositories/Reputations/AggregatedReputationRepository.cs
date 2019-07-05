using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;

namespace Plato.Internal.Repositories.Reputations
{
    
    public class AggregatedUserReputationRepository : IAggregatedUserReputationRepository
    {

        private readonly IDbHelper _dbHelper;

        public AggregatedUserReputationRepository(IDbHelper dbHelper)
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
                    {prefix}_UserReputations
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

        public async Task<AggregatedResult<int>> SelectSummedByIntAsync(
            string groupBy, 
            DateTimeOffset start,
            DateTimeOffset end)
        {

            // Sql query
            const string sql = @"             
                SELECT  
                    MAX(ur.{groupBy}) AS [Aggregate], 
                    SUM(ur.Points) AS [Count]
                FROM 
                    {prefix}_UserReputations ur INNER JOIN {prefix}_Users u ON ur.CreatedUserId = u.Id
                  WHERE 
                    ur.CreatedDate >= '{start}' AND ur.CreatedDate <= '{end}'
                GROUP BY 
                    ur.{groupBy}
                ORDER BY 
                    [Count] DESC
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
                var output = new AggregatedResult<int>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<int>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });
            
        }

        public async Task<AggregatedResult<int>> SelectSummedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end,
            int featureId)
        {
            // Sql query
            const string sql = @"             
                SELECT  
                    MAX(ur.{groupBy}) AS [Aggregate], 
                    SUM(ur.Points) AS [Count]
                FROM 
                    {prefix}_UserReputations ur INNER JOIN {prefix}_Users u ON ur.CreatedUserId = u.Id
                  WHERE (
                    (ur.CreatedDate >= '{start}' AND ur.CreatedDate <= '{end}') AND
                    (ur.FeatureId = {featureId})
                )
                GROUP BY 
                    ur.{groupBy}
                ORDER BY 
                    [Count] DESC
            ";

            // Sql replacements
            var replacements = new Dictionary<string, string>()
            {
                ["{groupBy}"] = groupBy,
                ["{start}"] = start.ToSortableDateTimePattern(),
                ["{end}"] = end.ToSortableDateTimePattern(),
                ["{featureId}"] = featureId.ToString()
            };

            // Execute and return results
            return await _dbHelper.ExecuteReaderAsync(sql, replacements, async reader =>
            {
                var output = new AggregatedResult<int>();
                while (await reader.ReadAsync())
                {
                    var aggregatedCount = new AggregatedCount<int>();
                    aggregatedCount.PopulateModel(reader);
                    output.Data.Add(aggregatedCount);
                }
                return output;
            });

        }
        
        // ----------------
        // Grouped by feature
        // ----------------

        public async Task<AggregatedResult<string>> SelectGroupedByFeature(DateTimeOffset start, DateTimeOffset end)
        {

            // Sql query
            const string sql = @"
                SELECT 
                    f.ModuleId AS [Aggregate] ,
                    COUNT(ur.Id) AS Count
                FROM 
                    {prefix}_UserReputations ur INNER JOIN {prefix}_ShellFeatures f ON f.Id = ur.FeatureId
                WHERE 
                    ur.CreatedDate >= '{start}' AND ur.CreatedDate <= '{end}'
                GROUP BY 
                    f.ModuleId
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

        // ----------------
        // Grouped by reputation name
        // ----------------

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByNameAsync(
            string reputationName,
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
                    {prefix}_UserReputations 
                WHERE 
                    [Name] = '{reputationName}' AND
                    {groupBy} >= '{start}' AND {groupBy} <= '{end}'
                GROUP BY 
                    YEAR({groupBy}),
                    MONTH({groupBy}), 
                    DAY({groupBy})
            ";

            // Sql replacements
            // Note user supplied input should never be passed into this method
            var replacements = new Dictionary<string, string>()
            {
                ["{reputationName}"] = reputationName.Replace("'", "''"),
                ["{groupBy}"] = groupBy.Replace("'", "''"),
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

    }

}
