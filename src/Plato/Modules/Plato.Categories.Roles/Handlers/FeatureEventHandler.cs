using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Roles.Services;
using Plato.Internal.Features.Abstractions;

namespace Plato.Categories.Roles.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultCategoryRolesManager<CategoryBase> _defaultCategoryRolesManager;

        public FeatureEventHandler(IDefaultCategoryRolesManager<CategoryBase> defaultCategoryRolesManager)
        {
            _defaultCategoryRolesManager = defaultCategoryRolesManager;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            await _defaultCategoryRolesManager.InstallAsync();
        }

    }

}
