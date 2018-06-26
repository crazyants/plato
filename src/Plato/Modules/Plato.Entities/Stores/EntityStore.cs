using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Stores
{

    public class EntityStore : IEntityStore<Entity>
    {
        private readonly IEntityRepository<Entity> _entityRepository;
        private readonly ILogger<EntityStore> _logger;

        public EntityStore(
            ILogger<EntityStore> logger,
            IEntityRepository<Entity> entityRepository)
        {
            _logger = logger;
            _entityRepository = entityRepository;
        }

        public async Task<Entity> CreateAsync(Entity entity)
        {

            return await _entityRepository.InsertUpdateAsync(entity);

        }

        public async Task<Entity> UpdateAsync(Entity entity)
        {
            return await _entityRepository.InsertUpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Entity entity)
        {
            var success = await _entityRepository.DeleteAsync(entity.Id);
            if (success)
            {
                //_cacheDependency.CancelToken(CacheKey.GetRolesByUserIdCacheKey(model.UserId));
            }

            return success;
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
            return await _entityRepository.SelectByIdAsync(id);
        }

        public IQuery QueryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IPagedResults<T>> SelectAsync<T>(params object[] args) where T : class
        {
            throw new NotImplementedException();
        }

    }

}
