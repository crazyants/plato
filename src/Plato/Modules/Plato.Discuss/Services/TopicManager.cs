using System.Threading.Tasks;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Discuss.Services
{
    public class TopicManager : IPostManager<Entity>
    {

        private readonly IEntityManager<Entity> _entityManager;
        private readonly IContextFacade _contextFacade;

        public TopicManager(IEntityManager<Entity> entityManager, 
            IContextFacade contextFacade)
        {
            _entityManager = entityManager;
            _contextFacade = contextFacade;
        }

        public async Task<IActivityResult<Entity>> CreateAsync(Entity model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _contextFacade.GetCurrentFeatureAsync();
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<IActivityResult<Entity>> UpdateAsync(Entity model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _contextFacade.GetCurrentFeatureAsync();
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<IActivityResult<Entity>> DeleteAsync(int id)
        {
            return await _entityManager.DeleteAsync(id);
        }
    }
}
