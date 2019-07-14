using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Repositories;

namespace Plato.Entities.Repositories
{

    public interface IAggregatedFeatureEntitiesRepository : IQueryableRepository<AggregatedResult<string>>
    {

    }

    public class AggregatedFeatureEntitiesRepository : IAggregatedFeatureEntitiesRepository
    {


        private readonly IDbContext _dbContext;

        public AggregatedFeatureEntitiesRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IPagedResults<AggregatedResult<string>>> SelectAsync(IDbDataParameter[] dbParams)
        {

            IPagedResults<AggregatedResult<string>> results = null;
            using (var context = _dbContext)
            {

                results = await context.ExecuteReaderAsync<IPagedResults<AggregatedResult<string>>>(
                    CommandType.StoredProcedure,
                    "SelectEntityUsersPaged",
                    async reader =>
                    {


                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new AggregatedResult<string>();
                            while (await reader.ReadAsync())
                            {
                                var aggregatedCount = new AggregatedCount<string>();
                                aggregatedCount.PopulateModel(reader);
                                output.Data.Add(aggregatedCount);
                            }

                            var pagedResults = new PagedResults<AggregatedResult<string>>
                            {
                                Data = new List<AggregatedResult<string>>() {output}
                            };

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                pagedResults.PopulateTotal(reader);
                            }

                            return pagedResults;

                        }

                        return null;
                    },
                    dbParams);

            }

            return results;

        }


    }

}
