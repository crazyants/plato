using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Services;

namespace Plato.Discuss.Services
{
    public class TopicManager : IPostManager<Entity>
    {

        private readonly IEntityManager<Entity> _entityManager;

        public TopicManager(IEntityManager<Entity> entityManager)
        {
            _entityManager = entityManager;
        }

        public async Task<IEntityResult> CreateAsync(Entity model)
        {
            return await _entityManager.CreateAsync(model);
        }

        public async Task<IEntityResult> UpdateAsync(Entity model)
        {
            return await _entityManager.UpdateAsync(model);
        }

        public async Task<IEntityResult> DeleteAsync(int id)
        {
            return await _entityManager.DeleteAsync(id);
        }
    }
}
