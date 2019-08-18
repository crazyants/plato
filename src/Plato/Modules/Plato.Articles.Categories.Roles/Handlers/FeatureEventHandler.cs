using System.Threading.Tasks;
using Plato.Categories.Roles.Services;
using Plato.Articles.Categories.Models;
using Plato.Internal.Features.Abstractions;

namespace Plato.Articles.Categories.Roles.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultCategoryRolesManager<Category> _defaultCategoryRolesManager;
        private readonly IFeatureFacade _featureFacade;

        public FeatureEventHandler(
            IDefaultCategoryRolesManager<Category> defaultCategoryRolesManager,
            IFeatureFacade featureFacade)
        {
            _defaultCategoryRolesManager = defaultCategoryRolesManager;
            _featureFacade = featureFacade;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {

            // Install default roles for Plato.Docs.Categories
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles.Categories");
            if (feature != null)
            {
                await _defaultCategoryRolesManager.InstallAsync(feature.Id);
            }
       
        }

    }

}
