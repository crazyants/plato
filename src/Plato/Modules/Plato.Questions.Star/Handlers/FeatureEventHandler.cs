using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Star.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
        
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(IDefaultRolesManager defaultRolesManager)
        {
            _defaultRolesManager = defaultRolesManager;
        }
        
        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

        public override async Task UpdatedAsync(IFeatureEventContext context)
        {
            // Apply any additional permissions to default roles for updated feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
        }

    }

}
