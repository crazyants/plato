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

    }

}
