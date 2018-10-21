using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Repositories;

namespace Plato.Entities.Stores
{
    

    public interface IEntityUsersStore
    {

        Task<IEnumerable<EntityUser>> GetUniqueUsers(EntityUserQueryParams queryParams);

    }

    public class EntityUsersStore : IEntityUsersStore
    {

        private readonly IEntityUsersRepository _entityUsersRepository;

        public EntityUsersStore(IEntityUsersRepository entityUsersRepository)
        {
            _entityUsersRepository = entityUsersRepository;
        }
        
        public async Task<IEnumerable<EntityUser>> GetUniqueUsers(EntityUserQueryParams queryParams)
        {
            return await _entityUsersRepository.GetUniqueUsers(queryParams);
        }

    }

}
