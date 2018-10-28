using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Discuss.Services
{
    public class TopicManager : IPostManager<Topic>
    {

        private readonly IEntityManager<Topic> _entityManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;


        public TopicManager(
            IEntityManager<Topic> entityManager, 
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Topic>> CreateAsync(Topic model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<ICommandResult<Topic>> UpdateAsync(Topic model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<ICommandResult<Topic>> DeleteAsync(Topic model)
        {
            return await _entityManager.DeleteAsync(model);
        }

    }
}
