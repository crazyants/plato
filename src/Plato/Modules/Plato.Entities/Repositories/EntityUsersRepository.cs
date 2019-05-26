using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Repositories
{
    
    public interface IEntityUsersRepository
    {
        Task<IPagedResults<EntityUser>> SelectAsync(IDbDataParameter[] dbParams);
    }

    public class EntityUsersRepository : IEntityUsersRepository
    {

        private readonly IDbContext _dbContext;
        private readonly ILogger<EntityUsersRepository> _logger;
        
        public EntityUsersRepository(
            IDbContext dbContext,
            ILogger<EntityUsersRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<IPagedResults<EntityUser>> SelectAsync(IDbDataParameter[] dbParams)
        {
            IPagedResults<EntityUser> results = null;
            using (var context = _dbContext)
            {

                results = await context.ExecuteReaderAsync2<IPagedResults<EntityUser>>(
                    CommandType.StoredProcedure,
                    "SelectEntityUsersPaged",
                    async reader =>
                    {
                        if ((reader != null) && (reader.HasRows))
                        {
                            var output = new PagedResults<EntityUser>();
                            while (await reader.ReadAsync())
                            {
                                var entity = new EntityUser();
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
