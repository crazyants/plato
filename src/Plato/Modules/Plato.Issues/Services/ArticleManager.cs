using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Issues.Services
{
    public class IssueManager : IPostManager<Issue>
    {

        private readonly IEntityManager<Issue> _entityManager;
        private readonly IFeatureFacade _featureFacade;
        
        public IssueManager(
            IEntityManager<Issue> entityManager, 
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Issue>> CreateAsync(Issue model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<ICommandResult<Issue>> UpdateAsync(Issue model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Issues");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<ICommandResult<Issue>> DeleteAsync(Issue model)
        {
            return await _entityManager.DeleteAsync(model);
        }

    }
}
