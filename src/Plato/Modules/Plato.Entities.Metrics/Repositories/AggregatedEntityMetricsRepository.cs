using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;

namespace Plato.Entities.Metrics.Repositories
{
    
    public class AggregatedEntityMetricsRepository : IAggregatedEntityMetricsRepository
    {

        private readonly IDbHelper _dbHelper;

        public AggregatedEntityMetricsRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDate(string groupBy, DateTimeOffset start, DateTimeOffset end)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityMetrics
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

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDate(string groupBy, DateTimeOffset start, DateTimeOffset end, int featureId)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(em.Id) AS [Count], 
                    MAX(em.{groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityMetrics em INNER JOIN {prefix}_Entities e ON e.Id = em.EntityId
                WHERE (
                    (em.{groupBy} >= '{start}' AND em.{groupBy} <= '{end}') AND
                    (e.FeatureId = {featureId})                    
                )
                GROUP BY 
                    YEAR(em.{groupBy}),
                    MONTH(em.{groupBy}), 
                    DAY(em.{groupBy})
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

        public async Task<AggregatedResult<int>> SelectGroupedByInt(string groupBy, DateTimeOffset start, DateTimeOffset end)
        {
            // Sql query
            const string sql = @"             
                SELECT                   
                    COUNT(em.Id) AS [Count], 
                    MAX(em.{groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityMetrics em INNER JOIN {prefix}_Entities e ON em.EntityId = e.Id
                WHERE 
                    em.CreatedDate >= '{start}' AND em.CreatedDate <= '{end}'                
                GROUP BY 
                    em.{groupBy}
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

        // --------------------

        public async Task<AggregatedResult<int>> SelectGroupedByInt(string groupBy, DateTimeOffset start, DateTimeOffset end, int featureId)
        {

            // Sql query
            const string sql = @"             
                SELECT                   
                    COUNT(em.Id) AS [Count], 
                    MAX(em.{groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityMetrics em INNER JOIN {prefix}_Entities e ON em.EntityId = e.Id
                WHERE (
                    (em.CreatedDate >= '{start}' AND em.CreatedDate <= '{end}') AND
                    (e.FeatureId = {featureId})
                )
                GROUP BY 
                    em.{groupBy}
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


    }

}
