using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;

namespace Plato.Metrics.Repositories
{
    
    public class AggregatedMetricsRepository : IAggregatedMetricsRepository
    {

        private readonly IDbHelper _dbHelper;

        public AggregatedMetricsRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDateAsync(string groupBy, DateTimeOffset start, DateTimeOffset end)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX({groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_Metrics
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


        // ----------------
        // Grouped by feature
        // ----------------

        public async Task<AggregatedResult<string>> SelectGroupedByFeature(DateTimeOffset start, DateTimeOffset end)
        {

            // Sql query
            const string sql = @"
                SELECT 
                    f.ModuleId AS [Aggregate] ,
                    COUNT(m.Id) AS Count
                FROM 
                    {prefix}_Metrics m INNER JOIN {prefix}_ShellFeatures f ON f.Id = m.FeatureId
                WHERE 
                    m.CreatedDate >= '{start}' AND m.CreatedDate <= '{end}'
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


    }

}
