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
        
        public async Task<AggregatedResult<DateTimeOffset>> SelectGroupedByDate(
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

        public Task<AggregatedResult<string>> SelectGroupedByFeature()
        {
            throw new NotImplementedException();
        }

        public Task<AggregatedResult<string>> SelectGroupedByFeature(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
