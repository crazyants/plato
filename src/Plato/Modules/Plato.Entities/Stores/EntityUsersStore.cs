using System.Data;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Abstractions;

namespace Plato.Entities.Stores
{

    public interface IEntityUsersStore : IQueryableStore2<EntityUser>
    {
    }

    public class EntityUsersStore : IEntityUsersStore
    {

        private readonly IEntityUsersRepository _entityUsersRepository;
        private readonly IDbQueryConfiguration _dbQuery;

        public EntityUsersStore(IEntityUsersRepository entityUsersRepository, IDbQueryConfiguration dbQuery)
        {
            _entityUsersRepository = entityUsersRepository;
            _dbQuery = dbQuery;
        }
        
        public IQuery<EntityUser> QueryAsync()
        {
            var query = new EntityUserQuery(this);
            return _dbQuery.ConfigureQuery<EntityUser>(query); ;
        }

        public async Task<IPagedResults<EntityUser>> SelectAsync(IDbDataParameter[] dbParams)
        {
            return await _entityUsersRepository.SelectAsync(dbParams);
        }

    }

}
