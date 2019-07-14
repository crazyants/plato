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

    public interface IAggregatedFeatureEntitiesRepository : IQueryableRepository<AggregatedCount<string>>
    {

    }

    public class AggregatedFeatureEntitiesRepository : IAggregatedFeatureEntitiesRepository
    {


        private readonly IDbContext _dbContext;

        public AggregatedFeatureEntitiesRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IPagedResults<AggregatedCount<string>>> SelectAsync(IDbDataParameter[] dbParams)
        {

            IPagedResults<AggregatedCount<string>> results = null;
            using (var context = _dbContext)
            {

                results = await context.ExecuteReaderAsync<IPagedResults<AggregatedCount<string>>>(
                    CommandType.StoredProcedure,
                    "SelectEntityUsersPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<AggregatedCount<string>>();
                            while (await reader.ReadAsync())
                            {
                                var aggregatedCount = new AggregatedCount<string>();
                                aggregatedCount.PopulateModel(reader);
                                output.Data.Add(aggregatedCount);
                            }

                            if (await reader.NextResultAsync())
                            {
                                await reader.ReadAsync();
                                output.PopulateTotal(reader);
                            }

                            return output;
                        }

                        return null;
                    },
                    dbParams);

            }

            return results;
            
        }


    }

}
