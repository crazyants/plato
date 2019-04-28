using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories.Metrics;

namespace Plato.Entities.Repositories
{
    
    public class AggregatedEntityReplyRepository : IAggregatedEntityReplyRepository
    {

        private readonly IDbHelper _dbHelper;

        public AggregatedEntityReplyRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // ----------------
        // Grouped by date
        // ----------------

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
                    {prefix}_EntityReplies
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
                    COUNT(er.Id) AS [Count], 
                    MAX(er.{groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityReplies er INNER JOIN {prefix}_Entities e ON e.Id = er.EntityId
                WHERE (
                    (er.{groupBy} >= '{start}' AND er.{groupBy} <= '{end}') AND
                    (e.FeatureId = {featureId})
                )
                GROUP BY 
                    YEAR(er.{groupBy}),
                    MONTH(er.{groupBy}), 
                    DAY(er.{groupBy})
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

        // ----------------
        // Grouped by feature
        // ----------------

        public Task<AggregatedResult<string>> SelectGroupedByFeature()
        {
            throw new NotImplementedException();
        }

        public Task<AggregatedResult<string>> SelectGroupedByFeature(int userId)
        {
            throw new NotImplementedException();
        }

        // ----------------
        // Grouped by int
        // ----------------

        public async Task<AggregatedResult<int>> SelectGroupedByIntAsync(
            string groupBy,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            // Sql query
            const string sql = @"
                SELECT 
                    COUNT(Id) AS [Count], 
                    MAX(er.{groupBy}) AS [Aggregate] 
                FROM 
                    {prefix}_EntityReplies er
                WHERE 
                    er.CreatedDate >= '{start}' AND er.CreatedDate <= '{end}'
                GROUP BY 
                    er.{groupBy}
                ORDER BY [Count] DESC
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



    }
}
