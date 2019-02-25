using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Entities.Services;
using Plato.Internal.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Articles.Services
{
    public class ArticleManager : IPostManager<Article>
    {

        private readonly IEntityManager<Article> _entityManager;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;


        public ArticleManager(
            IEntityManager<Article> entityManager, 
            IContextFacade contextFacade,
            IFeatureFacade featureFacade)
        {
            _entityManager = entityManager;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
        }

        public async Task<ICommandResult<Article>> CreateAsync(Article model)
        {
            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.CreateAsync(model);
        }

        public async Task<ICommandResult<Article>> UpdateAsync(Article model)
        {

            if (model.FeatureId == 0)
            {
                var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");
                if (feature != null)
                {
                    model.FeatureId = feature.Id;
                }
            }

            return await _entityManager.UpdateAsync(model);
        }

        public async Task<ICommandResult<Article>> DeleteAsync(Article model)
        {
            return await _entityManager.DeleteAsync(model);
        }

    }
}
