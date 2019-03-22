using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Questions.Services
{
    public class QuestionManager : IPostManager<Question>
    {

        private readonly IEntityManager<Question> _entityManager;
        private readonly IFeatureFacade _featureFacade;
        
        public QuestionManager(
            IEntityManager<Question> entityManager, 
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Question>> CreateAsync(Question model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<ICommandResult<Question>> UpdateAsync(Question model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Questions");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<ICommandResult<Question>> DeleteAsync(Question model)
        {
            return await _entityManager.DeleteAsync(model);
        }

    }
}
