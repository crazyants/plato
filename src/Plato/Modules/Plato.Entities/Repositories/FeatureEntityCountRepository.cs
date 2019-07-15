using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{

    public class FeatureEntityCountRepository : IFeatureEntityCountRepository
    {
        
        private readonly IDbContext _dbContext;

        public FeatureEntityCountRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IPagedResults<FeatureEntityCount>> SelectAsync(IDbDataParameter[] dbParams)
        {

            IPagedResults<FeatureEntityCount> results = null;
            using (var context = _dbContext)
            {

                results = await context.ExecuteReaderAsync(
                    CommandType.StoredProcedure,
                    "SelectEntitiesPaged",
                    async reader =>
                    {

                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<FeatureEntityCount>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new FeatureEntityCount();
                                entity.PopulateModel(reader);
                                output.Data.Add(entity);
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
