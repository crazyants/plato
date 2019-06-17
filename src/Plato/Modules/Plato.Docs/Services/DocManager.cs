using System.Linq;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Docs.Services
{

    public class DocManager : IPostManager<Doc>
    {

        private readonly IEntityStore<Doc> _entityStore;
        private readonly IEntityManager<Doc> _entityManager;
        private readonly IFeatureFacade _featureFacade;
        
        public DocManager(
            IEntityManager<Doc> entityManager,
            IEntityStore<Doc> entityStore,
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _entityStore = entityStore;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Doc>> CreateAsync(Doc model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }
            
            return await _entityManager.CreateAsync(model);

        }

        public async Task<ICommandResult<Doc>> UpdateAsync(Doc model)
        {

            // We always need a feature
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }
            
            return await _entityManager.UpdateAsync(model);

        }

        public async Task<ICommandResult<Doc>> DeleteAsync(Doc model)
        {
            return await _entityManager.DeleteAsync(model);
        }

        public async Task<ICommandResult<Doc>> Move(Doc model, MoveDirection direction)
        {
            return await _entityManager.Move(model, direction);
        }
        
    }

}
