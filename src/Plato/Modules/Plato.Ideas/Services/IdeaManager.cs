using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Ideas.Services
{
    public class IdeaManager : IPostManager<Idea>
    {

        private readonly IEntityManager<Idea> _entityManager;
        private readonly IFeatureFacade _featureFacade;
        
        public IdeaManager(
            IEntityManager<Idea> entityManager, 
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Idea>> CreateAsync(Idea model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<ICommandResult<Idea>> UpdateAsync(Idea model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<ICommandResult<Idea>> DeleteAsync(Idea model)
        {
            return await _entityManager.DeleteAsync(model);
        }

    }
}
